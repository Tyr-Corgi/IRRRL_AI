namespace IRRRL.Core.Enums;

/// <summary>
/// Type of IRRRL application
/// </summary>
public enum ApplicationType
{
    /// <summary>
    /// Rate-and-term refinance only (primary focus - streamlined processing)
    /// </summary>
    RateAndTerm = 0,
    
    /// <summary>
    /// Cash-out refinance (case-by-case, requires manual review and full documentation)
    /// </summary>
    CashOut = 1
}

