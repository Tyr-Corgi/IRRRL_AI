namespace IRRRL.Core.Entities;

/// <summary>
/// Net Tangible Benefit calculation results
/// </summary>
public class NetTangibleBenefit : BaseEntity
{
    public int IRRRLApplicationId { get; set; }
    public IRRRLApplication IRRRLApplication { get; set; } = null!;
    
    // Current loan details
    public decimal CurrentInterestRate { get; set; }
    public decimal CurrentMonthlyPayment { get; set; }
    public int CurrentRemainingTermMonths { get; set; }
    
    // New loan details
    public decimal NewInterestRate { get; set; }
    public decimal NewMonthlyPayment { get; set; }
    public int NewTermMonths { get; set; }
    
    // Calculations
    public decimal InterestRateReduction { get; set; }
    public decimal MonthlyPaymentSavings { get; set; }
    public decimal TotalLoanCosts { get; set; }
    public int RecoupmentPeriodMonths { get; set; }
    
    // Savings over time
    public decimal TotalSavingsOverRemainingTerm { get; set; }
    public decimal TotalInterestSavings { get; set; }
    
    // Equity and term analysis
    public int TermReductionMonths { get; set; } // Negative if term increases
    public decimal EquityGrowthAcceleration { get; set; }
    
    // NTB test results
    public bool MeetsRecoupmentRequirement { get; set; } // Must be <= 36 months
    public bool MeetsInterestRateRequirement { get; set; } // >= 0.5% for fixed-to-fixed
    public bool MeetsPaymentReductionRequirement { get; set; }
    public bool PassesNTBTest { get; set; }
    
    // Additional analysis
    public decimal BreakEvenMonths { get; set; }
    public string? Notes { get; set; }
    
    // Calculation metadata
    public DateTime CalculatedDate { get; set; } = DateTime.UtcNow;
    public string CalculatedBy { get; set; } = "System";
}

