using IRRRL.Core.Entities;
using IRRRL.Core.Enums;

namespace IRRRL.Core.Interfaces;

/// <summary>
/// Service for managing application workflow and status transitions
/// </summary>
public interface IApplicationWorkflowService
{
    /// <summary>
    /// Transition application to a new status
    /// </summary>
    Task<bool> TransitionToStatusAsync(IRRRLApplication application, ApplicationStatus newStatus, string? userId = null, string? notes = null);
    
    /// <summary>
    /// Check if status transition is valid
    /// </summary>
    bool CanTransitionTo(ApplicationStatus currentStatus, ApplicationStatus newStatus);
    
    /// <summary>
    /// Get valid next statuses for current status
    /// </summary>
    List<ApplicationStatus> GetValidNextStatuses(ApplicationStatus currentStatus);
    
    /// <summary>
    /// Start AI analysis for new application
    /// </summary>
    Task StartAIAnalysisAsync(IRRRLApplication application);
    
    /// <summary>
    /// Complete AI analysis and move to next stage
    /// </summary>
    Task CompleteAIAnalysisAsync(IRRRLApplication application);
    
    /// <summary>
    /// Start document gathering phase
    /// </summary>
    Task StartDocumentGatheringAsync(IRRRLApplication application);
    
    /// <summary>
    /// Complete document gathering and start AI processing
    /// </summary>
    Task CompleteDocumentGatheringAsync(IRRRLApplication application);
    
    /// <summary>
    /// Prepare file for underwriter
    /// </summary>
    Task PrepareForUnderwriterAsync(IRRRLApplication application);
}

