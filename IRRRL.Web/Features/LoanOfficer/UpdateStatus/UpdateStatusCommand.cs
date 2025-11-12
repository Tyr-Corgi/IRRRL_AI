using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Core.Grains;
using IRRRL.Infrastructure.Data;
using IRRRL.Web.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Orleans;

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
/// NOW USING ORLEANS GRAINS!
/// 
/// VERTICAL SLICE + ORLEANS PATTERN:
/// 1. Vertical Slice provides the structure (Feature folder with Command + Handler)
/// 2. Handler delegates to Orleans Grain for actual work
/// 3. Grain handles state management, concurrency, and notifications
/// 4. Result: Clean architecture + distributed scalability
/// </summary>
public class UpdateStatusHandler : IRequestHandler<UpdateStatusCommand, Result>
{
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<UpdateStatusHandler> _logger;

    public UpdateStatusHandler(
        IGrainFactory grainFactory,
        ILogger<UpdateStatusHandler> logger)
    {
        _grainFactory = grainFactory;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UpdateStatusCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating status for application {ApplicationId} to {NewStatus}",
            request.ApplicationId,
            request.NewStatus);

        try
        {
            // Get the ApplicationGrain (Orleans automatically activates it if needed)
            var applicationGrain = _grainFactory.GetGrain<IApplicationGrain>(request.ApplicationId);
            
            // Call the Grain to update status
            // The Grain handles:
            // - State management
            // - Concurrency (thread-safe)
            // - Database updates
            // - Notifications to other users
            await applicationGrain.UpdateStatusAsync(request.NewStatus, request.Notes);

            _logger.LogInformation(
                "Successfully delegated status update to ApplicationGrain {ApplicationId}",
                request.ApplicationId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating status for application {ApplicationId}",
                request.ApplicationId);
            return Result.Failure($"Failed to update status: {ex.Message}");
        }
    }
}

