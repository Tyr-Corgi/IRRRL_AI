using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Core.Grains;
using IRRRL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orleans;

namespace IRRRL.Infrastructure.Grains;

/// <summary>
/// Implementation of IApplicationGrain
/// This Grain manages a single IRRRL application's state and operations
/// 
/// KEY ORLEANS CONCEPTS DEMONSTRATED:
/// 1. Grain Activation - OnActivateAsync() loads state when grain is first accessed
/// 2. Thread-Safe Operations - Orleans guarantees one message at a time
/// 3. State Management - Uses EF Core for persistence
/// 4. Automatic Distribution - Orleans decides which Silo hosts this grain
/// 5. Grain-to-Grain Calls - Can call other grains (e.g., NotificationGrain)
/// </summary>
public class ApplicationGrain : Grain, IApplicationGrain
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ILogger<ApplicationGrain> _logger;
    private readonly IGrainFactory _grainFactory;
    
    private IRRRLApplication? _application;
    private int ApplicationId => (int)this.GetPrimaryKeyLong();

    public ApplicationGrain(
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        ILogger<ApplicationGrain> logger,
        IGrainFactory grainFactory)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _grainFactory = grainFactory;
    }

    /// <summary>
    /// Called when the Grain is activated (first access or after deactivation)
    /// This is where we load the application state from the database
    /// </summary>
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Activating ApplicationGrain for Application ID: {ApplicationId}", ApplicationId);
        
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        _application = await context.IRRRLApplications
            .Include(a => a.Borrower)
            .Include(a => a.Property)
            .Include(a => a.CurrentLoan)
            .Include(a => a.Documents)
            .Include(a => a.ActionItems)
            .Include(a => a.Notes)
            .FirstOrDefaultAsync(a => a.Id == ApplicationId, cancellationToken);
        
        if (_application == null)
        {
            _logger.LogWarning("Application {ApplicationId} not found in database", ApplicationId);
        }
        else
        {
            _logger.LogInformation(
                "ApplicationGrain activated for {ApplicationNumber} (Status: {Status})",
                _application.ApplicationNumber,
                _application.Status);
        }
        
        await base.OnActivateAsync(cancellationToken);
    }

    public Task<IRRRLApplication?> GetApplicationAsync()
    {
        return Task.FromResult(_application);
    }

    public async Task UpdateStatusAsync(ApplicationStatus newStatus, string? notes = null, string? changedByUserId = null)
    {
        if (_application == null)
        {
            _logger.LogWarning("Cannot update status - application not loaded");
            return;
        }

        var oldStatus = _application.Status;
        
        if (oldStatus == newStatus)
        {
            _logger.LogDebug("Status unchanged for application {ApplicationNumber}", _application.ApplicationNumber);
            return;
        }

        _logger.LogInformation(
            "Updating status for {ApplicationNumber}: {OldStatus} → {NewStatus}",
            _application.ApplicationNumber,
            oldStatus,
            newStatus);

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        // Update status
        _application.Status = newStatus;
        _application.UpdatedAt = DateTime.UtcNow;
        
        // Add status history
        var historyEntry = new ApplicationStatusHistory
        {
            IRRRLApplicationId = _application.Id,
            FromStatus = oldStatus,
            ToStatus = newStatus,
            ChangedAt = DateTime.UtcNow,
            ChangedByUserId = changedByUserId,
            Notes = notes
        };
        
        context.Attach(_application);
        context.Entry(_application).State = EntityState.Modified;
        context.ApplicationStatusHistories.Add(historyEntry);
        
        await context.SaveChangesAsync();
        
        // Notify loan officer (Grain-to-Grain communication!)
        if (!string.IsNullOrEmpty(_application.AssignedLoanOfficerId))
        {
            var notificationGrain = _grainFactory.GetGrain<INotificationGrain>(_application.AssignedLoanOfficerId);
            await notificationGrain.SendNotificationAsync(
                "Application Status Updated",
                $"Application {_application.ApplicationNumber} status changed to {newStatus}",
                NotificationType.ApplicationUpdate);
        }
        
        _logger.LogInformation("Status updated successfully for {ApplicationNumber}", _application.ApplicationNumber);
    }

    public async Task AddNoteAsync(string content, ApplicationNoteType noteType, bool isImportant, string createdByUserId, string createdByName)
    {
        if (_application == null)
        {
            _logger.LogWarning("Cannot add note - application not loaded");
            return;
        }

        _logger.LogInformation("Adding note to application {ApplicationNumber}", _application.ApplicationNumber);

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var note = new ApplicationNote
        {
            IRRRLApplicationId = _application.Id,
            Content = content,
            NoteType = noteType,
            IsImportant = isImportant,
            CreatedByUserId = createdByUserId,
            CreatedByName = createdByName,
            CreatedAt = DateTime.UtcNow
        };
        
        context.ApplicationNotes.Add(note);
        await context.SaveChangesAsync();
        
        // Update local state
        _application.Notes.Add(note);
        
        _logger.LogInformation("Note added to application {ApplicationNumber}", _application.ApplicationNumber);
    }

    public async Task MarkDocumentReceivedAsync(int documentId)
    {
        if (_application == null)
        {
            _logger.LogWarning("Cannot mark document - application not loaded");
            return;
        }

        _logger.LogInformation("Marking document {DocumentId} as received", documentId);

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var document = await context.Documents.FindAsync(documentId);
        if (document != null && document.IRRRLApplicationId == _application.Id)
        {
            document.IsComplete = true;
            document.ValidatedDate = DateTime.UtcNow;
            document.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Document {DocumentId} marked as received", documentId);
        }
    }

    public async Task CompleteActionItemAsync(int actionItemId)
    {
        if (_application == null)
        {
            _logger.LogWarning("Cannot complete action item - application not loaded");
            return;
        }

        _logger.LogInformation("Completing action item {ActionItemId}", actionItemId);

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var actionItem = await context.ActionItems.FindAsync(actionItemId);
        if (actionItem != null && actionItem.IRRRLApplicationId == _application.Id)
        {
            actionItem.Status = ActionItemStatus.Completed;
            actionItem.CompletedDate = DateTime.UtcNow;
            actionItem.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Action item {ActionItemId} completed", actionItemId);
        }
    }

    public Task<ApplicationStatisticsDto> GetStatisticsAsync()
    {
        if (_application == null)
        {
            return Task.FromResult(new ApplicationStatisticsDto(0, 0, 0, ApplicationStatus.Submitted, DateTime.UtcNow));
        }

        var stats = new ApplicationStatisticsDto(
            DocumentsNeededCount: _application.Documents.Count(d => !d.IsComplete),
            OpenActionItemsCount: _application.ActionItems.Count(ai => ai.Status != ActionItemStatus.Completed),
            NotesCount: _application.Notes.Count,
            CurrentStatus: _application.Status,
            LastUpdated: _application.UpdatedAt ?? _application.CreatedAt
        );

        return Task.FromResult(stats);
    }
}

