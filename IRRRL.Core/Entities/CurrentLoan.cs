using IRRRL.Core.Enums;

namespace IRRRL.Core.Entities;

/// <summary>
/// Current VA loan information
/// </summary>
public class CurrentLoan : BaseEntity
{
    public string LoanNumber { get; set; } = string.Empty;
    public string Lender { get; set; } = string.Empty;
    public LoanType LoanType { get; set; }
    
    // Loan details
    public decimal CurrentBalance { get; set; }
    public decimal OriginalLoanAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int RemainingTermMonths { get; set; }
    public int OriginalTermMonths { get; set; }
    public DateTime OriginationDate { get; set; }
    
    // Monthly payment breakdown
    public decimal MonthlyPrincipalAndInterest { get; set; }
    public decimal MonthlyPropertyTax { get; set; }
    public decimal MonthlyInsurance { get; set; }
    public decimal MonthlyPMI { get; set; }
    public decimal TotalMonthlyPayment { get; set; }
    
    // Payment history
    public bool CurrentOnPayments { get; set; }
    public int LatePaymentsLast12Months { get; set; }
    public int LatePaymentsOver30Days { get; set; }
    public DateTime? LastLatePaymentDate { get; set; }
    
    // VA loan specific
    public bool IsVALoan { get; set; } = true;
    public decimal OriginalVAFundingFee { get; set; }
    
    // Navigation
    public int IRRRLApplicationId { get; set; }
    public IRRRLApplication IRRRLApplication { get; set; } = null!;
}

