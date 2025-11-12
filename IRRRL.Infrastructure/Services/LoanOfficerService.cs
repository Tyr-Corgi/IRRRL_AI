using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Core.Interfaces;
using IRRRL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IRRRL.Infrastructure.Services;

/// <summary>
/// Service for loan officer operations
/// </summary>
public class LoanOfficerService : ILoanOfficerService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LoanOfficerService> _logger;

    public LoanOfficerService(ApplicationDbContext context, ILogger<LoanOfficerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<IRRRLApplication>> GetAllApplicationsAsync(string? loanOfficerId = null)
    {
        try
        {
            var query = _context.IRRRLApplications
                .Include(a => a.Borrower)
                .Include(a => a.Property)
                .Include(a => a.CurrentLoan)
                .Include(a => a.ActionItems)
                .AsQueryable();

            if (!string.IsNullOrEmpty(loanOfficerId))
            {
                query = query.Where(a => a.AssignedLoanOfficerId == loanOfficerId);
            }

            return await query
                .OrderByDescending(a => a.SubmittedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading applications for loan officer {LoanOfficerId}", loanOfficerId);
            return new List<IRRRLApplication>();
        }
    }

    public async Task<IRRRLApplication?> GetApplicationDetailAsync(int applicationId)
    {
        try
        {
            return await _context.IRRRLApplications
                .Include(a => a.Borrower)
                .Include(a => a.Property)
                .Include(a => a.CurrentLoan)
                .Include(a => a.Documents)
                .Include(a => a.ActionItems)
                .Include(a => a.Notes)
                .Include(a => a.NetTangibleBenefitCalculation)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading application detail for {ApplicationId}", applicationId);
            return null;
        }
    }

    public async Task<bool> UpdateApplicationStatusAsync(int applicationId, string newStatus, string? note = null, string? userId = null)
    {
        try
        {
            var application = await _context.IRRRLApplications.FindAsync(applicationId);
            if (application == null)
            {
                _logger.LogWarning("Application {ApplicationId} not found", applicationId);
                return false;
            }

            var oldStatus = application.Status;
            
            // Parse and update status
            if (Enum.TryParse<ApplicationStatus>(newStatus, out var status))
            {
                application.Status = status;
            }
            else
            {
                _logger.LogWarning("Invalid status {Status}", newStatus);
                return false;
            }

            // Add status history
            var statusHistory = new ApplicationStatusHistory
            {
                IRRRLApplicationId = applicationId,
                FromStatus = oldStatus,
                ToStatus = status,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = userId,
                Notes = note
            };
            _context.ApplicationStatusHistories.Add(statusHistory);

            // Add note if provided
            if (!string.IsNullOrWhiteSpace(note) && !string.IsNullOrWhiteSpace(userId))
            {
                var appNote = new ApplicationNote
                {
                    IRRRLApplicationId = applicationId,
                    NoteType = ApplicationNoteType.StatusChange,
                    Content = $"Status changed from {oldStatus} to {status}. Note: {note}",
                    IsImportant = false,
                    CreatedByUserId = userId,
                    CreatedByName = "Loan Officer" // TODO: Get actual name
                };
                _context.ApplicationNotes.Add(appNote);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Application {ApplicationId} status updated from {OldStatus} to {NewStatus}", 
                applicationId, oldStatus, status);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating application status for {ApplicationId}", applicationId);
            return false;
        }
    }

    public async Task<ApplicationNote> AddNoteAsync(int applicationId, string noteType, string content, bool isImportant, string userId, string userName)
    {
        try
        {
            var note = new ApplicationNote
            {
                IRRRLApplicationId = applicationId,
                NoteType = Enum.Parse<ApplicationNoteType>(noteType),
                Content = content,
                IsImportant = isImportant,
                CreatedByUserId = userId,
                CreatedByName = userName
            };

            _context.ApplicationNotes.Add(note);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Note added to application {ApplicationId} by {UserName}", applicationId, userName);
            return note;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding note to application {ApplicationId}", applicationId);
            throw;
        }
    }

    public async Task<List<ApplicationNote>> GetApplicationNotesAsync(int applicationId)
    {
        try
        {
            return await _context.ApplicationNotes
                .Where(n => n.IRRRLApplicationId == applicationId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading notes for application {ApplicationId}", applicationId);
            return new List<ApplicationNote>();
        }
    }

    public async Task<bool> DeleteNoteAsync(int noteId, string userId)
    {
        try
        {
            var note = await _context.ApplicationNotes.FindAsync(noteId);
            if (note == null)
            {
                return false;
            }

            // Only allow deletion by creator or admin
            if (note.CreatedByUserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to delete note {NoteId} created by {CreatorId}", 
                    userId, noteId, note.CreatedByUserId);
                return false;
            }

            _context.ApplicationNotes.Remove(note);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Note {NoteId} deleted by {UserId}", noteId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note {NoteId}", noteId);
            return false;
        }
    }

    public async Task<bool> ToggleActionItemAsync(int actionItemId, string userId)
    {
        try
        {
            var actionItem = await _context.ActionItems.FindAsync(actionItemId);
            if (actionItem == null)
            {
                return false;
            }

            // Toggle completion
            if (actionItem.Status == ActionItemStatus.Completed)
            {
                actionItem.Status = ActionItemStatus.Pending;
                actionItem.CompletedDate = null;
                actionItem.CompletedByUserId = null;
            }
            else
            {
                actionItem.Status = ActionItemStatus.Completed;
                actionItem.CompletedDate = DateTime.UtcNow;
                actionItem.CompletedByUserId = userId;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Action item {ActionItemId} toggled to {Status}", actionItemId, actionItem.Status);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling action item {ActionItemId}", actionItemId);
            return false;
        }
    }

    public async Task<ActionItem> AddActionItemAsync(int applicationId, string title, string description, string priority, string? userId = null)
    {
        try
        {
            var actionItem = new ActionItem
            {
                IRRRLApplicationId = applicationId,
                Title = title,
                Description = description,
                Priority = Enum.Parse<ActionItemPriority>(priority),
                Status = ActionItemStatus.Pending,
                GeneratedByAI = false,
                AssignedToUserId = userId
            };

            _context.ActionItems.Add(actionItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Manual action item added to application {ApplicationId}", applicationId);
            return actionItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding action item to application {ApplicationId}", applicationId);
            throw;
        }
    }

    public async Task<bool> DeleteActionItemAsync(int actionItemId)
    {
        try
        {
            var actionItem = await _context.ActionItems.FindAsync(actionItemId);
            if (actionItem == null)
            {
                return false;
            }

            _context.ActionItems.Remove(actionItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Action item {ActionItemId} deleted", actionItemId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting action item {ActionItemId}", actionItemId);
            return false;
        }
    }

    public async Task<bool> ToggleDocumentReceivedAsync(int documentId)
    {
        try
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null)
            {
                return false;
            }

            document.IsValidated = !document.IsValidated;
            document.ValidatedDate = document.IsValidated ? DateTime.UtcNow : null;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Document {DocumentId} validation toggled", documentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling document {DocumentId}", documentId);
            return false;
        }
    }

    public async Task<ApplicationStatistics> GetStatisticsAsync(string? loanOfficerId = null)
    {
        try
        {
            var query = _context.IRRRLApplications.AsQueryable();

            if (!string.IsNullOrEmpty(loanOfficerId))
            {
                query = query.Where(a => a.AssignedLoanOfficerId == loanOfficerId);
            }

            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var stats = new ApplicationStatistics
            {
                NewCount = await query.CountAsync(a => a.Status == ApplicationStatus.Submitted),
                InProgressCount = await query.CountAsync(a => 
                    a.Status == ApplicationStatus.DocumentGathering || 
                    a.Status == ApplicationStatus.AIProcessing ||
                    a.Status == ApplicationStatus.FilePreparation),
                ReadyForReviewCount = await query.CountAsync(a => a.Status == ApplicationStatus.UnderwriterReady),
                CompletedLast30DaysCount = await query.CountAsync(a => 
                    (a.Status == ApplicationStatus.Closed || a.Status == ApplicationStatus.Approved) &&
                    a.CompletedDate.HasValue && a.CompletedDate.Value >= thirtyDaysAgo)
            };

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading statistics for loan officer {LoanOfficerId}", loanOfficerId);
            return new ApplicationStatistics();
        }
    }
}

