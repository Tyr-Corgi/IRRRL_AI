using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Infrastructure.Data;
using IRRRL.Web.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IRRRL.Web.Features.Underwriter.GetQueue;

/// <summary>
/// Query to get underwriter queue (applications ready for underwriting)
/// This is Underwriter-specific - completely independent from LoanOfficer and Veteran features
/// </summary>
public record GetQueueQuery(string? UnderwriterId = null) : IQuery<GetQueueResult>;

/// <summary>
/// Result containing underwriter queue
/// Different data than LoanOfficer dashboard - shows underwriter-specific info
/// </summary>
public record GetQueueResult(
    List<UnderwriterQueueItem> QueueItems,
    QueueStatistics Statistics
);

/// <summary>
/// DTO for underwriter queue item
/// Notice: Different fields than LoanOfficer dashboard!
/// Each role sees what THEY need - no shared view models
/// </summary>
public record UnderwriterQueueItem(
    int ApplicationId,
    string ApplicationNumber,
    string VeteranName,
    decimal CurrentLoanBalance,
    decimal RequestedLoanAmount,
    decimal InterestRateReduction,
    decimal MonthlySavings,
    bool MeetsNTB,
    int RecoupmentMonths,
    DateTime SubmittedToUnderwriting,
    int DaysInQueue,
    bool HasAllDocuments,
    string Priority
);

public record QueueStatistics(
    int TotalInQueue,
    int ReadyForReview,
    int PendingDocuments,
    int AverageProcessingDays
);

/// <summary>
/// Handler for GetQueueQuery
/// Completely different logic from LoanOfficer dashboard
/// Shows: Vertical Slice = Different roles = Different features = Different code
/// </summary>
public class GetQueueHandler : IRequestHandler<GetQueueQuery, GetQueueResult>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetQueueHandler> _logger;

    public GetQueueHandler(
        ApplicationDbContext context,
        ILogger<GetQueueHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetQueueResult> Handle(
        GetQueueQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Loading underwriter queue for: {UnderwriterId}",
            request.UnderwriterId ?? "All");

        // Query applications ready for underwriting
        var query = _context.IRRRLApplications
            .Include(a => a.Borrower)
            .Include(a => a.CurrentLoan)
            .Include(a => a.Documents)
            .Where(a => 
                a.Status == ApplicationStatus.UnderwriterReady ||
                a.Status == ApplicationStatus.InUnderwriting)
            .AsQueryable();

        // Note: AssignedUnderwriterId not yet in schema - would filter if it existed
        // if (!string.IsNullOrEmpty(request.UnderwriterId))
        // {
        //     query = query.Where(a => a.AssignedUnderwriterId == request.UnderwriterId);
        // }

        var applications = await query
            .OrderBy(a => a.SubmittedDate) // FIFO queue by submission date
            .ToListAsync(cancellationToken);

        // Project to underwriter-specific DTOs
        var queueItems = applications.Select(a => new UnderwriterQueueItem(
            ApplicationId: a.Id,
            ApplicationNumber: a.ApplicationNumber,
            VeteranName: $"{a.Borrower.FirstName} {a.Borrower.LastName}",
            CurrentLoanBalance: a.CurrentLoan?.CurrentBalance ?? 0,
            RequestedLoanAmount: a.RequestedLoanAmount,
            InterestRateReduction: (a.CurrentLoan?.InterestRate ?? 0) - a.RequestedInterestRate,
            MonthlySavings: a.EstimatedMonthlySavings,
            MeetsNTB: a.MeetsNetTangibleBenefit,
            RecoupmentMonths: a.RecoupmentPeriodMonths,
            SubmittedToUnderwriting: a.SubmittedDate ?? a.CreatedAt,
            DaysInQueue: a.SubmittedDate.HasValue 
                ? (DateTime.UtcNow - a.SubmittedDate.Value).Days 
                : (DateTime.UtcNow - a.CreatedAt).Days,
            HasAllDocuments: a.Documents.All(d => d.IsComplete),
            Priority: DeterminePriority(a)
        )).ToList();

        // Calculate underwriter-specific statistics
        var statistics = new QueueStatistics(
            TotalInQueue: queueItems.Count,
            ReadyForReview: queueItems.Count(q => q.HasAllDocuments),
            PendingDocuments: queueItems.Count(q => !q.HasAllDocuments),
            AverageProcessingDays: queueItems.Any() 
                ? (int)queueItems.Average(q => q.DaysInQueue) 
                : 0
        );

        _logger.LogInformation(
            "Loaded {QueueCount} applications for underwriting",
            queueItems.Count);

        return new GetQueueResult(queueItems, statistics);
    }

    private static string DeterminePriority(IRRRLApplication app)
    {
        // Business logic for priority - underwriter-specific!
        var submittedDate = app.SubmittedDate ?? app.CreatedAt;
        var daysWaiting = (DateTime.UtcNow - submittedDate).Days;

        if (daysWaiting > 5) return "HIGH";
        if (daysWaiting > 2) return "MEDIUM";
        return "NORMAL";
    }
}

