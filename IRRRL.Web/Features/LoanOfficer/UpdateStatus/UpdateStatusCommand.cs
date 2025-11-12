using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Infrastructure.Data;
using IRRRL.Web.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IRRRL.Web.Features.LoanOfficer.UpdateStatus;

/// <summary>
/// Command to update application status
/// Creates audit trail in status history
/// </summary>
public record UpdateStatusCommand(
    int ApplicationId,
    ApplicationStatus NewStatus,
    string? Notes = null
) : ICommand<Result>;

/// <summary>
/// Handler for UpdateStatusCommand
/// Updates status and creates history record
/// </summary>
public class UpdateStatusHandler : IRequestHandler<UpdateStatusCommand, Result>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UpdateStatusHandler> _logger;

    public UpdateStatusHandler(
        ApplicationDbContext context,
        ILogger<UpdateStatusHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UpdateStatusCommand request,
        CancellationToken cancellationToken)
    {
        var application = await _context.IRRRLApplications
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken);

        if (application == null)
        {
            _logger.LogWarning(
                "Attempted to update status for non-existent application: {ApplicationId}",
                request.ApplicationId);
            return Result.Failure($"Application {request.ApplicationId} not found");
        }

        // Check if status is actually changing
        if (application.Status == request.NewStatus)
        {
            _logger.LogInformation(
                "Status unchanged for application {ApplicationNumber}: {Status}",
                application.ApplicationNumber,
                request.NewStatus);
            return Result.Success();
        }

        var oldStatus = application.Status;
        
        // Update application status
        application.Status = request.NewStatus;
        application.UpdatedAt = DateTime.UtcNow;

        // Add status history record
        var historyEntry = new ApplicationStatusHistory
        {
            IRRRLApplicationId = application.Id,
            FromStatus = oldStatus,
            ToStatus = request.NewStatus,
            ChangedAt = DateTime.UtcNow,
            Notes = request.Notes
        };
        _context.ApplicationStatusHistories.Add(historyEntry);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Application {ApplicationNumber} status updated from {OldStatus} to {NewStatus}",
            application.ApplicationNumber,
            oldStatus,
            request.NewStatus);

        return Result.Success();
    }
}

