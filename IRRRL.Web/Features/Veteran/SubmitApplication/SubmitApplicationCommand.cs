using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Infrastructure.Data;
using IRRRL.Web.Features.Common;
using MediatR;

namespace IRRRL.Web.Features.Veteran.SubmitApplication;

/// <summary>
/// Command to submit a new IRRRL application
/// This is a Veteran-specific feature - completely independent from LoanOfficer features
/// </summary>
public record SubmitApplicationCommand(
    string UserId,
    IRRRLApplication Application
) : ICommand<Result<string>>; // Returns application number

/// <summary>
/// Handler for SubmitApplicationCommand
/// Validates, saves application, and triggers notifications
/// Notice: This has ZERO dependencies on LoanOfficer features!
/// </summary>
public class SubmitApplicationHandler 
    : IRequestHandler<SubmitApplicationCommand, Result<string>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubmitApplicationHandler> _logger;

    public SubmitApplicationHandler(
        ApplicationDbContext context,
        ILogger<SubmitApplicationHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(
        SubmitApplicationCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Veteran {UserId} submitting new application",
            request.UserId);

        // Generate application number
        var applicationCount = await _context.IRRRLApplications.CountAsync(cancellationToken);
        var applicationNumber = $"IRRRL-{DateTime.UtcNow.Year}-{(applicationCount + 1):D3}";

        // Set application details
        request.Application.ApplicationNumber = applicationNumber;
        request.Application.Status = ApplicationStatus.Submitted;
        request.Application.SubmittedDate = DateTime.UtcNow;
        request.Application.CreatedAt = DateTime.UtcNow;

        // Save to database
        _context.IRRRLApplications.Add(request.Application);
        
        // Add initial status history
        _context.ApplicationStatusHistories.Add(new ApplicationStatusHistory
        {
            IRRRLApplication = request.Application,
            OldStatus = ApplicationStatus.Draft,
            NewStatus = ApplicationStatus.Submitted,
            ChangedAt = DateTime.UtcNow,
            Notes = "Application submitted by veteran"
        });

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Application {ApplicationNumber} submitted successfully",
            applicationNumber);

        // In a real app, you'd trigger notifications here
        // await _notificationService.NotifyLoanOfficersOfNewApplication(applicationNumber);

        return Result.Success(applicationNumber);
    }
}

