using IRRRL.Core.Enums;

namespace IRRRL.Core.Entities;

/// <summary>
/// IRRRL refinance application
/// </summary>
public class IRRRLApplication : BaseEntity
{
    public string ApplicationNumber { get; set; } = string.Empty; // Auto-generated unique identifier
    
    // Application type and status
    public ApplicationType ApplicationType { get; set; } = ApplicationType.RateAndTerm;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;
    
    // Borrower and property
    public int BorrowerId { get; set; }
    public Borrower Borrower { get; set; } = null!;
    
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    
    // Current loan
    public CurrentLoan? CurrentLoan { get; set; }
    
    // New loan details
    public decimal RequestedLoanAmount { get; set; }
    public decimal RequestedInterestRate { get; set; }
    public int RequestedTermMonths { get; set; } = 360; // Default 30 years
    public LoanType NewLoanType { get; set; } = LoanType.FixedRate;
    
    // Cash-out (if applicable)
    public decimal? CashOutAmount { get; set; }
    public string? CashOutPurpose { get; set; }
    
    // Calculated values
    public decimal EstimatedNewMonthlyPayment { get; set; }
    public decimal EstimatedMonthlySavings { get; set; }
    public int RecoupmentPeriodMonths { get; set; }
    public bool MeetsNetTangibleBenefit { get; set; }
    
    // Funding fee
    public decimal FundingFeePercentage { get; set; } = 0.5m;
    public decimal FundingFeeAmount { get; set; }
    public bool FundingFeeWaived { get; set; }
    
    // Closing costs
    public decimal TotalClosingCosts { get; set; }
    public decimal TotalLoanCosts { get; set; }
    
    // Eligibility
    public bool EligibilityVerified { get; set; }
    public string? EligibilityNotes { get; set; }
    
    // Approval/decline
    public bool? IsApproved { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovedBy { get; set; }
    public string? DeclineReason { get; set; }
    
    // Assignment
    public string? AssignedLoanOfficerId { get; set; }
    public string? AssignedUnderwriterId { get; set; }
    
    // Important dates
    public DateTime? SubmittedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? EstimatedClosingDate { get; set; }
    public DateTime? ActualClosingDate { get; set; }
    
    // Navigation properties
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<ActionItem> ActionItems { get; set; } = new List<ActionItem>();
    public ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
    public NetTangibleBenefit? NetTangibleBenefitCalculation { get; set; }
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}

