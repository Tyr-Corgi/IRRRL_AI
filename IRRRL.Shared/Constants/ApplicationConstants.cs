namespace IRRRL.Shared.Constants;

/// <summary>
/// Application-wide constants for IRRRL processing
/// </summary>
public static class ApplicationConstants
{
    /// <summary>
    /// VA IRRRL requirements
    /// </summary>
    public static class VARequirements
    {
        /// <summary>
        /// Maximum recoupment period in months (VA requirement)
        /// </summary>
        public const int MaxRecoupmentPeriodMonths = 36;
        
        /// <summary>
        /// Minimum interest rate reduction for fixed-to-fixed (percentage points)
        /// </summary>
        public const decimal MinInterestRateReduction = 0.5m;
        
        /// <summary>
        /// Default funding fee percentage for IRRRL
        /// </summary>
        public const decimal DefaultFundingFeePercentage = 0.5m;
        
        /// <summary>
        /// Maximum cash-out amount without full documentation (energy improvements)
        /// </summary>
        public const decimal MaxCashOutWithoutFullDocs = 6000m;
        
        /// <summary>
        /// Maximum late payments allowed in last 6 months
        /// </summary>
        public const int MaxLatePaymentsLast6Months = 0;
        
        /// <summary>
        /// Maximum 30+ day late payments allowed in last 12 months
        /// </summary>
        public const int MaxLatePayments30DaysLast12Months = 0;
    }
    
    /// <summary>
    /// User roles in the system
    /// </summary>
    public static class Roles
    {
        public const string Veteran = "Veteran";
        public const string LoanOfficer = "LoanOfficer";
        public const string Underwriter = "Underwriter";
        public const string Administrator = "Administrator";
    }
    
    /// <summary>
    /// Document-related constants
    /// </summary>
    public static class Documents
    {
        public const long MaxFileSizeBytes = 10485760; // 10 MB
        public static readonly string[] AllowedFileTypes = { ".pdf", ".jpg", ".jpeg", ".png", ".tif", ".tiff" };
        public const int DocumentExpirationDays = 90; // Pay stubs, etc.
    }
    
    /// <summary>
    /// AI processing constants
    /// </summary>
    public static class AI
    {
        public const decimal MinConfidenceScore = 0.7m;
        public const int MaxRetries = 3;
        public const int TimeoutSeconds = 30;
    }
}

