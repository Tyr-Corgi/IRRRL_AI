using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Core.Interfaces;
using IRRRL.Shared.Constants;

namespace IRRRL.Core.Services;

/// <summary>
/// Implements Net Tangible Benefit calculations for VA IRRRL
/// </summary>
public class NetTangibleBenefitCalculator : INetTangibleBenefitCalculator
{
    public NetTangibleBenefit Calculate(IRRRLApplication application)
    {
        if (application.CurrentLoan == null)
        {
            throw new InvalidOperationException("Current loan information is required for NTB calculation");
        }
        
        var currentLoan = application.CurrentLoan;
        
        // Calculate new monthly payment
        var newMonthlyPayment = CalculateMonthlyPayment(
            application.RequestedLoanAmount,
            application.RequestedInterestRate,
            application.RequestedTermMonths
        );
        
        // Calculate current monthly payment (P&I only for NTB comparison)
        var currentMonthlyPayment = currentLoan.MonthlyPrincipalAndInterest;
        
        // Calculate savings
        var monthlyPaymentSavings = currentMonthlyPayment - newMonthlyPayment;
        var interestRateReduction = currentLoan.InterestRate - application.RequestedInterestRate;
        
        // Calculate recoupment period
        var recoupmentPeriodMonths = CalculateRecoupmentPeriod(
            application.TotalLoanCosts,
            monthlyPaymentSavings
        );
        
        // Calculate total interest savings
        var currentTotalInterest = CalculateTotalInterest(
            currentLoan.CurrentBalance,
            currentMonthlyPayment,
            currentLoan.RemainingTermMonths
        );
        
        var newTotalInterest = CalculateTotalInterest(
            application.RequestedLoanAmount,
            newMonthlyPayment,
            application.RequestedTermMonths
        );
        
        var totalInterestSavings = currentTotalInterest - newTotalInterest;
        
        // Calculate total savings over remaining term
        var remainingTermForComparison = Math.Min(currentLoan.RemainingTermMonths, application.RequestedTermMonths);
        var totalSavingsOverRemainingTerm = monthlyPaymentSavings * remainingTermForComparison;
        
        // Calculate term difference
        var termReductionMonths = currentLoan.RemainingTermMonths - application.RequestedTermMonths;
        
        // Calculate equity growth acceleration (simplified)
        // This is the extra principal paid monthly due to lower interest
        var equityGrowthAcceleration = CalculateEquityGrowthAcceleration(
            currentLoan.CurrentBalance,
            currentMonthlyPayment,
            currentLoan.InterestRate,
            newMonthlyPayment,
            application.RequestedInterestRate
        );
        
        // Create NTB entity
        var ntb = new NetTangibleBenefit
        {
            IRRRLApplicationId = application.Id,
            
            // Current loan
            CurrentInterestRate = currentLoan.InterestRate,
            CurrentMonthlyPayment = currentMonthlyPayment,
            CurrentRemainingTermMonths = currentLoan.RemainingTermMonths,
            
            // New loan
            NewInterestRate = application.RequestedInterestRate,
            NewMonthlyPayment = newMonthlyPayment,
            NewTermMonths = application.RequestedTermMonths,
            
            // Calculations
            InterestRateReduction = interestRateReduction,
            MonthlyPaymentSavings = monthlyPaymentSavings,
            TotalLoanCosts = application.TotalLoanCosts,
            RecoupmentPeriodMonths = recoupmentPeriodMonths,
            
            // Savings
            TotalSavingsOverRemainingTerm = totalSavingsOverRemainingTerm,
            TotalInterestSavings = totalInterestSavings,
            
            // Term and equity
            TermReductionMonths = termReductionMonths,
            EquityGrowthAcceleration = equityGrowthAcceleration,
            
            // Break-even
            BreakEvenMonths = monthlyPaymentSavings > 0 
                ? application.TotalLoanCosts / monthlyPaymentSavings 
                : 0,
            
            // Test results
            MeetsRecoupmentRequirement = recoupmentPeriodMonths <= ApplicationConstants.VARequirements.MaxRecoupmentPeriodMonths,
            MeetsInterestRateRequirement = CheckInterestRateRequirement(currentLoan.LoanType, application.NewLoanType, interestRateReduction),
            MeetsPaymentReductionRequirement = monthlyPaymentSavings > 0 || (currentLoan.LoanType == LoanType.ARM && application.NewLoanType == LoanType.FixedRate),
            
            CalculatedDate = DateTime.UtcNow,
            CalculatedBy = "System"
        };
        
        // Determine if passes overall NTB test
        ntb.PassesNTBTest = MeetsNTBRequirements(ntb, application);
        
        return ntb;
    }
    
    public decimal CalculateMonthlyPayment(decimal loanAmount, decimal annualInterestRate, int termMonths)
    {
        if (loanAmount <= 0 || termMonths <= 0)
        {
            return 0;
        }
        
        if (annualInterestRate == 0)
        {
            return loanAmount / termMonths;
        }
        
        // Convert annual rate to monthly rate (as decimal)
        var monthlyRate = (double)(annualInterestRate / 100 / 12);
        var principal = (double)loanAmount;
        var numberOfPayments = termMonths;
        
        // Calculate monthly payment using formula: P * [r(1+r)^n] / [(1+r)^n - 1]
        var poweredRate = Math.Pow(1 + monthlyRate, numberOfPayments);
        var monthlyPayment = principal * (monthlyRate * poweredRate) / (poweredRate - 1);
        
        return (decimal)Math.Round(monthlyPayment, 2);
    }
    
    public int CalculateRecoupmentPeriod(decimal totalLoanCosts, decimal monthlyPaymentSavings)
    {
        if (monthlyPaymentSavings <= 0 || totalLoanCosts <= 0)
        {
            return int.MaxValue; // Cannot recoup
        }
        
        var months = totalLoanCosts / monthlyPaymentSavings;
        return (int)Math.Ceiling(months);
    }
    
    public decimal CalculateTotalInterest(decimal loanAmount, decimal monthlyPayment, int termMonths)
    {
        if (loanAmount <= 0 || monthlyPayment <= 0 || termMonths <= 0)
        {
            return 0;
        }
        
        var totalPaid = monthlyPayment * termMonths;
        var totalInterest = totalPaid - loanAmount;
        
        return Math.Max(0, totalInterest);
    }
    
    public bool MeetsNTBRequirements(NetTangibleBenefit ntb, IRRRLApplication application)
    {
        // Must meet recoupment requirement (36 months or less)
        if (!ntb.MeetsRecoupmentRequirement)
        {
            return false;
        }
        
        // Must meet interest rate requirement
        if (!ntb.MeetsInterestRateRequirement)
        {
            return false;
        }
        
        // Must have payment reduction OR moving from ARM to fixed-rate
        if (!ntb.MeetsPaymentReductionRequirement)
        {
            return false;
        }
        
        return true;
    }
    
    private bool CheckInterestRateRequirement(LoanType currentLoanType, LoanType newLoanType, decimal interestRateReduction)
    {
        // Fixed-rate to fixed-rate must have at least 0.5% reduction
        if (currentLoanType == LoanType.FixedRate && newLoanType == LoanType.FixedRate)
        {
            return interestRateReduction >= ApplicationConstants.VARequirements.MinInterestRateReduction;
        }
        
        // ARM to fixed-rate provides stability benefit, so rate requirement may be waived
        if (currentLoanType == LoanType.ARM && newLoanType == LoanType.FixedRate)
        {
            return true; // Stability is the benefit
        }
        
        // Other combinations should have some rate reduction
        return interestRateReduction > 0;
    }
    
    private decimal CalculateEquityGrowthAcceleration(
        decimal currentBalance,
        decimal currentMonthlyPayment,
        decimal currentInterestRate,
        decimal newMonthlyPayment,
        decimal newInterestRate)
    {
        // Calculate first month's interest for both loans
        var currentMonthlyInterestRate = currentInterestRate / 100 / 12;
        var newMonthlyInterestRate = newInterestRate / 100 / 12;
        
        var currentFirstMonthInterest = currentBalance * currentMonthlyInterestRate;
        var currentFirstMonthPrincipal = currentMonthlyPayment - currentFirstMonthInterest;
        
        var newFirstMonthInterest = currentBalance * newMonthlyInterestRate;
        var newFirstMonthPrincipal = newMonthlyPayment - newFirstMonthInterest;
        
        // Difference in principal payment is the equity growth acceleration
        return newFirstMonthPrincipal - currentFirstMonthPrincipal;
    }
}

