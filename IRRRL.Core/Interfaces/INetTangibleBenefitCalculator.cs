using IRRRL.Core.Entities;

namespace IRRRL.Core.Interfaces;

/// <summary>
/// Service for calculating Net Tangible Benefit for IRRRL applications
/// </summary>
public interface INetTangibleBenefitCalculator
{
    /// <summary>
    /// Calculate Net Tangible Benefit for an IRRRL application
    /// </summary>
    NetTangibleBenefit Calculate(IRRRLApplication application);
    
    /// <summary>
    /// Calculate monthly payment for a loan
    /// </summary>
    decimal CalculateMonthlyPayment(decimal loanAmount, decimal annualInterestRate, int termMonths);
    
    /// <summary>
    /// Calculate recoupment period in months
    /// </summary>
    int CalculateRecoupmentPeriod(decimal totalLoanCosts, decimal monthlyPaymentSavings);
    
    /// <summary>
    /// Calculate total interest paid over the life of the loan
    /// </summary>
    decimal CalculateTotalInterest(decimal loanAmount, decimal monthlyPayment, int termMonths);
    
    /// <summary>
    /// Determine if application meets VA NTB requirements
    /// </summary>
    bool MeetsNTBRequirements(NetTangibleBenefit ntb, IRRRLApplication application);
}

