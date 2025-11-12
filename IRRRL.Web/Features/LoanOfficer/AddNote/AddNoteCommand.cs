using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Infrastructure.Data;
using IRRRL.Web.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IRRRL.Web.Features.LoanOfficer.AddNote;

/// <summary>
/// Command to add a note to an application
/// Commands change state - they have side effects
/// </summary>
public record AddNoteCommand(
    int ApplicationId,
    string Content,
    ApplicationNoteType NoteType,
    bool IsImportant,
    string CreatedByUserId,
    string CreatedByName
) : ICommand<Result<ApplicationNote>>;

/// <summary>
/// Handler for AddNoteCommand
/// Validates and adds note to database
/// </summary>
public class AddNoteHandler : IRequestHandler<AddNoteCommand, Result<ApplicationNote>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AddNoteHandler> _logger;

    public AddNoteHandler(
        ApplicationDbContext context,
        ILogger<AddNoteHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<ApplicationNote>> Handle(
        AddNoteCommand request,
        CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            _logger.LogWarning("Attempted to add note with empty content");
            return Result.Failure<ApplicationNote>("Note content cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(request.CreatedByUserId))
        {
            _logger.LogWarning("Attempted to add note without user ID");
            return Result.Failure<ApplicationNote>("User ID is required");
        }

        // Verify application exists
        var applicationExists = await _context.IRRRLApplications
            .AnyAsync(a => a.Id == request.ApplicationId, cancellationToken);

        if (!applicationExists)
        {
            _logger.LogWarning(
                "Attempted to add note to non-existent application: {ApplicationId}",
                request.ApplicationId);
            return Result.Failure<ApplicationNote>(
                $"Application {request.ApplicationId} not found");
        }

        // Create note
        var note = new ApplicationNote
        {
            IRRRLApplicationId = request.ApplicationId,
            Content = request.Content,
            NoteType = request.NoteType,
            IsImportant = request.IsImportant,
            CreatedByUserId = request.CreatedByUserId,
            CreatedByName = request.CreatedByName,
            CreatedAt = DateTime.UtcNow
        };

        _context.ApplicationNotes.Add(note);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Note added to application {ApplicationId} by {UserName}",
            request.ApplicationId,
            request.CreatedByName);

        return Result.Success(note);
    }
}

