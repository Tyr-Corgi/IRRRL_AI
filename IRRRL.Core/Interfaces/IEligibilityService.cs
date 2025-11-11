using IRRRL.Core.Entities;

namespace IRRRL.Core.Interfaces;

/// <summary>
/// Service for verifying VA IRRRL eligibility
/// </summary>
public interface IEligibilityService
{
    /// <summary>
    /// Verify eligibility for an IRRRL application
    /// </summary>
    EligibilityResult VerifyEligibility(IRRRLApplication application);
    
    /// <summary>
    /// Check if borrower has existing VA loan
    /// </summary>
    bool HasExistingVALoan(IRRRLApplication application);
    
    /// <summary>
    /// Check if borrower is current on payments
    /// </summary>
    bool IsCurrentOnPayments(CurrentLoan currentLoan);
    
    /// <summary>
    /// Check if property meets occupancy requirements
    /// </summary>
    bool MeetsOccupancyRequirements(Property property);
    
    /// <summary>
    /// Check if application meets Net Tangible Benefit test
    /// </summary>
    bool MeetsNetTangibleBenefit(NetTangibleBenefit ntb);
}

/// <summary>
/// Result of eligibility verification
/// </summary>
public class EligibilityResult
{
    public bool IsEligible { get; set; }
    public List<string> PassedChecks { get; set; } = new();
    public List<string> FailedChecks { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public string? Notes { get; set; }
}

