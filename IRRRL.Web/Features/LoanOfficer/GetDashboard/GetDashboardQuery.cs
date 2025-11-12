using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Infrastructure.Data;
using IRRRL.Web.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IRRRL.Web.Features.LoanOfficer.GetDashboard;

/// <summary>
/// Query to get loan officer dashboard data
/// Returns applications list and statistics
/// </summary>
public record GetDashboardQuery(string? LoanOfficerId = null) : IQuery<GetDashboardResult>;

/// <summary>
/// Result containing dashboard data
/// Using record for immutability and concise syntax
/// </summary>
public record GetDashboardResult(
    List<ApplicationSummaryDto> Applications,
    DashboardStatistics Statistics
);

/// <summary>
/// DTO for application summary (only what the dashboard needs)
/// Keeps network payload small and focused
/// </summary>
public record ApplicationSummaryDto(
    int Id,
    string ApplicationNumber,
    string VeteranName,
    string PhoneNumber,
    string PropertyAddress,
    string PropertyCity,
    string PropertyState,
    string PropertyZip,
    decimal CurrentRate,
    decimal CurrentMonthlyPayment,
    string LoanNumber,
    ApplicationStatus Status,
    DateTime SubmittedDate,
    int OpenActionItemsCount,
    int ActionItemsCount  // For backward compatibility with UI
);

/// <summary>
/// Statistics for dashboard cards
/// </summary>
public record DashboardStatistics(
    int NewCount,
    int InProgressCount,
    int ReadyForReviewCount,
    int CompletedLast30DaysCount
);

/// <summary>
/// Handler for GetDashboardQuery
/// ALL logic for this feature lives here - no service layer!
/// </summary>
public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, GetDashboardResult>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetDashboardHandler> _logger;

    public GetDashboardHandler(
        ApplicationDbContext context,
        ILogger<GetDashboardHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetDashboardResult> Handle(
        GetDashboardQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Loading dashboard for loan officer: {LoanOfficerId}", 
            request.LoanOfficerId ?? "All");

        // Build query
        var query = _context.IRRRLApplications
            .Include(a => a.Borrower)
            .Include(a => a.Property)
            .Include(a => a.CurrentLoan)
            .Include(a => a.ActionItems)
            .AsQueryable();

        // Filter by loan officer if specified
        if (!string.IsNullOrEmpty(request.LoanOfficerId))
        {
            query = query.Where(a => a.AssignedLoanOfficerId == request.LoanOfficerId);
        }

        // Load applications
        var applications = await query
            .OrderByDescending(a => a.SubmittedDate)
            .ToListAsync(cancellationToken);

        // Project to DTOs
        var openActionItemsCount = 0;
        var applicationDtos = applications.Select(a =>
        {
            openActionItemsCount = a.ActionItems.Count(ai => ai.Status != ActionItemStatus.Completed);
            return new ApplicationSummaryDto(
                Id: a.Id,
                ApplicationNumber: a.ApplicationNumber,
                VeteranName: $"{a.Borrower.FirstName} {a.Borrower.LastName}",
                PhoneNumber: a.Borrower.Phone,
                PropertyAddress: a.Property.StreetAddress,
                PropertyCity: a.Property.City,
                PropertyState: a.Property.State,
                PropertyZip: a.Property.ZipCode,
                CurrentRate: a.CurrentLoan?.InterestRate ?? 0,
                CurrentMonthlyPayment: a.CurrentLoan?.TotalMonthlyPayment ?? 0,
                LoanNumber: a.CurrentLoan?.LoanNumber ?? "",
                Status: a.Status,
                SubmittedDate: a.SubmittedDate ?? a.CreatedAt,
                OpenActionItemsCount: openActionItemsCount,
                ActionItemsCount: openActionItemsCount  // Same value for compatibility
            );
        }).ToList();

        // Calculate statistics
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var statistics = new DashboardStatistics(
            NewCount: applications.Count(a => a.Status == ApplicationStatus.Submitted),
            InProgressCount: applications.Count(a =>
                a.Status == ApplicationStatus.DocumentGathering ||
                a.Status == ApplicationStatus.AIProcessing ||
                a.Status == ApplicationStatus.FilePreparation),
            ReadyForReviewCount: applications.Count(a => a.Status == ApplicationStatus.UnderwriterReady),
            CompletedLast30DaysCount: applications.Count(a =>
                (a.Status == ApplicationStatus.Closed || a.Status == ApplicationStatus.Approved) &&
                a.CompletedDate.HasValue && a.CompletedDate.Value >= thirtyDaysAgo)
        );

        _logger.LogInformation(
            "Loaded {ApplicationCount} applications with {NewCount} new",
            applicationDtos.Count,
            statistics.NewCount);

        return new GetDashboardResult(applicationDtos, statistics);
    }
}

