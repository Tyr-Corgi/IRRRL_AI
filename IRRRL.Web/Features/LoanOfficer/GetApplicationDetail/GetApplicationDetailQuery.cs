using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Infrastructure.Data;
using IRRRL.Web.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IRRRL.Web.Features.LoanOfficer.GetApplicationDetail;

/// <summary>
/// Query to get full application details for review
/// </summary>
public record GetApplicationDetailQuery(int ApplicationId) : IQuery<GetApplicationDetailResult?>;

/// <summary>
/// Complete application details with all related data
/// </summary>
public record GetApplicationDetailResult(
    IRRRLApplication Application,
    int DocumentsNeededCount,
    int OpenActionItemsCount,
    int NotesCount
);

/// <summary>
/// Handler for GetApplicationDetailQuery
/// Loads complete application with all related entities
/// </summary>
public class GetApplicationDetailHandler 
    : IRequestHandler<GetApplicationDetailQuery, GetApplicationDetailResult?>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetApplicationDetailHandler> _logger;

    public GetApplicationDetailHandler(
        ApplicationDbContext context,
        ILogger<GetApplicationDetailHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetApplicationDetailResult?> Handle(
        GetApplicationDetailQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Loading application detail for ID: {ApplicationId}",
            request.ApplicationId);

        var application = await _context.IRRRLApplications
            .Include(a => a.Borrower)
            .Include(a => a.Property)
            .Include(a => a.CurrentLoan)
            .Include(a => a.Documents)
            .Include(a => a.ActionItems)
            .Include(a => a.Notes)
            .Include(a => a.StatusHistory)
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken);

        if (application == null)
        {
            _logger.LogWarning(
                "Application not found: {ApplicationId}",
                request.ApplicationId);
            return null;
        }

        // Calculate counts for badges
        var documentsNeededCount = application.Documents.Count(d => !d.IsComplete);
        var openActionItemsCount = application.ActionItems.Count(
            ai => ai.Status != ActionItemStatus.Completed);
        var notesCount = application.Notes.Count;

        _logger.LogInformation(
            "Loaded application {ApplicationNumber} with {DocumentsNeeded} docs needed, {OpenActions} open actions",
            application.ApplicationNumber,
            documentsNeededCount,
            openActionItemsCount);

        return new GetApplicationDetailResult(
            application,
            documentsNeededCount,
            openActionItemsCount,
            notesCount);
    }
}

