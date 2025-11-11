namespace IRRRL.Core.Enums;

/// <summary>
/// Current status of an IRRRL application in the workflow
/// </summary>
public enum ApplicationStatus
{
    /// <summary>
    /// Initial submission by veteran
    /// </summary>
    Submitted = 0,
    
    /// <summary>
    /// AI is performing initial analysis and generating action items
    /// </summary>
    AIAnalyzing = 1,
    
    /// <summary>
    /// Waiting for loan officer approval (cash-out applications only)
    /// </summary>
    PendingApproval = 2,
    
    /// <summary>
    /// Loan officer is gathering required documents
    /// </summary>
    DocumentGathering = 3,
    
    /// <summary>
    /// AI is processing documents and validating data
    /// </summary>
    AIProcessing = 4,
    
    /// <summary>
    /// AI is preparing final file package for underwriter
    /// </summary>
    FilePreparation = 5,
    
    /// <summary>
    /// Ready for underwriter review
    /// </summary>
    UnderwriterReady = 6,
    
    /// <summary>
    /// Under review by underwriter
    /// </summary>
    InUnderwriting = 7,
    
    /// <summary>
    /// Approved and ready for closing
    /// </summary>
    Approved = 8,
    
    /// <summary>
    /// Application declined
    /// </summary>
    Declined = 9,
    
    /// <summary>
    /// Application cancelled by veteran or loan officer
    /// </summary>
    Cancelled = 10,
    
    /// <summary>
    /// Loan has been closed
    /// </summary>
    Closed = 11
}

