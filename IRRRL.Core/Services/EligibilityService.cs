using IRRRL.Core.Entities;
using IRRRL.Core.Interfaces;
using IRRRL.Shared.Constants;

namespace IRRRL.Core.Services;

/// <summary>
/// Implements VA IRRRL eligibility verification
/// </summary>
public class EligibilityService : IEligibilityService
{
    public EligibilityResult VerifyEligibility(IRRRLApplication application)
    {
        var result = new EligibilityResult();
        
        // Check 1: Must have existing VA loan
        if (HasExistingVALoan(application))
        {
            result.PassedChecks.Add("Has existing VA loan");
        }
        else
        {
            result.FailedChecks.Add("No existing VA loan found - IRRRL requires refinancing an existing VA loan");
        }
        
        // Check 2: Must be current on payments
        if (application.CurrentLoan != null)
        {
            if (IsCurrentOnPayments(application.CurrentLoan))
            {
                result.PassedChecks.Add("Current on mortgage payments");
            }
            else
            {
                result.FailedChecks.Add($"Payment history issues: {application.CurrentLoan.LatePaymentsLast12Months} late payments in last 12 months, {application.CurrentLoan.LatePaymentsOver30Days} over 30 days late");
            }
        }
        else
        {
            result.FailedChecks.Add("Current loan information not provided");
        }
        
        // Check 3: Must meet occupancy requirements
        if (MeetsOccupancyRequirements(application.Property))
        {
            result.PassedChecks.Add("Meets occupancy requirements (previously or currently occupied)");
        }
        else
        {
            result.FailedChecks.Add("Property must have been previously or currently occupied by the veteran");
        }
        
        // Check 4: Must meet Net Tangible Benefit test
        if (application.NetTangibleBenefitCalculation != null)
        {
            if (MeetsNetTangibleBenefit(application.NetTangibleBenefitCalculation))
            {
                result.PassedChecks.Add("Meets Net Tangible Benefit requirements");
                
                // Add specific details
                var ntb = application.NetTangibleBenefitCalculation;
                result.PassedChecks.Add($"  - Monthly savings: ${ntb.MonthlyPaymentSavings:N2}");
                result.PassedChecks.Add($"  - Interest rate reduction: {ntb.InterestRateReduction:N3}%");
                result.PassedChecks.Add($"  - Recoupment period: {ntb.RecoupmentPeriodMonths} months (must be ≤36)");
            }
            else
            {
                result.FailedChecks.Add("Does not meet Net Tangible Benefit requirements");
                
                var ntb = application.NetTangibleBenefitCalculation;
                if (!ntb.MeetsRecoupmentRequirement)
                {
                    result.FailedChecks.Add($"  - Recoupment period of {ntb.RecoupmentPeriodMonths} months exceeds 36-month maximum");
                }
                if (!ntb.MeetsInterestRateRequirement)
                {
                    result.FailedChecks.Add($"  - Interest rate reduction of {ntb.InterestRateReduction:N3}% is less than required 0.5% for fixed-to-fixed");
                }
                if (!ntb.MeetsPaymentReductionRequirement)
                {
                    result.FailedChecks.Add("  - No monthly payment reduction or stability benefit");
                }
            }
        }
        else
        {
            result.Warnings.Add("Net Tangible Benefit calculation not yet performed");
        }
        
        // Additional warnings for cash-out applications
        if (application.ApplicationType == Enums.ApplicationType.CashOut)
        {
            result.Warnings.Add("Cash-out refinance requires manual review and full income documentation");
            
            if (application.CashOutAmount > ApplicationConstants.VARequirements.MaxCashOutWithoutFullDocs)
            {
                result.Warnings.Add($"Cash-out amount of ${application.CashOutAmount:N2} exceeds ${ApplicationConstants.VARequirements.MaxCashOutWithoutFullDocs:N2} - full documentation required");
            }
        }
        
        // Check if funding fee can be waived
        if (application.Borrower.HasDisabilityRating && application.Borrower.DisabilityPercentage >= 10)
        {
            result.PassedChecks.Add($"Funding fee waived due to {application.Borrower.DisabilityPercentage}% disability rating");
        }
        
        // Determine overall eligibility
        result.IsEligible = result.FailedChecks.Count == 0;
        
        // Add summary notes
        if (result.IsEligible)
        {
            result.Notes = "Application meets all VA IRRRL eligibility requirements.";
        }
        else
        {
            result.Notes = $"Application has {result.FailedChecks.Count} eligibility issue(s) that must be addressed.";
        }
        
        return result;
    }
    
    public bool HasExistingVALoan(IRRRLApplication application)
    {
        return application.CurrentLoan?.IsVALoan ?? false;
    }
    
    public bool IsCurrentOnPayments(CurrentLoan currentLoan)
    {
        // Must be current on payments
        if (!currentLoan.CurrentOnPayments)
        {
            return false;
        }
        
        // Check late payment history
        // No late payments in last 6 months
        // No 30+ day late payments in last 12 months
        var hasRecentLatePayments = currentLoan.LastLatePaymentDate.HasValue &&
                                   currentLoan.LastLatePaymentDate.Value > DateTime.UtcNow.AddMonths(-6);
        
        if (hasRecentLatePayments)
        {
            return false;
        }
        
        if (currentLoan.LatePaymentsOver30Days > ApplicationConstants.VARequirements.MaxLatePayments30DaysLast12Months)
        {
            return false;
        }
        
        return true;
    }
    
    public bool MeetsOccupancyRequirements(Property property)
    {
        // Property must have been previously or currently occupied by the veteran
        return property.CurrentlyOccupied || property.PreviouslyOccupied;
    }
    
    public bool MeetsNetTangibleBenefit(NetTangibleBenefit ntb)
    {
        return ntb.PassesNTBTest;
    }
}

