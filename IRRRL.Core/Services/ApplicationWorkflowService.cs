using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Core.Interfaces;

namespace IRRRL.Core.Services;

/// <summary>
/// Manages application workflow state transitions
/// </summary>
public class ApplicationWorkflowService : IApplicationWorkflowService
{
    private readonly Dictionary<ApplicationStatus, List<ApplicationStatus>> _validTransitions = new()
    {
        [ApplicationStatus.Submitted] = new() 
        { 
            ApplicationStatus.AIAnalyzing, 
            ApplicationStatus.PendingApproval, // Cash-out applications
            ApplicationStatus.Cancelled 
        },
        
        [ApplicationStatus.AIAnalyzing] = new() 
        { 
            ApplicationStatus.DocumentGathering,
            ApplicationStatus.Declined, // If eligibility fails
            ApplicationStatus.Cancelled 
        },
        
        [ApplicationStatus.PendingApproval] = new() 
        { 
            ApplicationStatus.AIAnalyzing, // If approved
            ApplicationStatus.Declined, // If rejected
            ApplicationStatus.Cancelled 
        },
        
        [ApplicationStatus.DocumentGathering] = new() 
        { 
            ApplicationStatus.AIProcessing,
            ApplicationStatus.Cancelled 
        },
        
        [ApplicationStatus.AIProcessing] = new() 
        { 
            ApplicationStatus.FilePreparation,
            ApplicationStatus.DocumentGathering, // If more docs needed
            ApplicationStatus.Cancelled 
        },
        
        [ApplicationStatus.FilePreparation] = new() 
        { 
            ApplicationStatus.UnderwriterReady,
            ApplicationStatus.DocumentGathering, // If issues found
            ApplicationStatus.Cancelled 
        },
        
        [ApplicationStatus.UnderwriterReady] = new() 
        { 
            ApplicationStatus.InUnderwriting,
            ApplicationStatus.Cancelled 
        },
        
        [ApplicationStatus.InUnderwriting] = new() 
        { 
            ApplicationStatus.Approved,
            ApplicationStatus.Declined,
            ApplicationStatus.DocumentGathering, // If more docs needed
            ApplicationStatus.Cancelled 
        },
        
        [ApplicationStatus.Approved] = new() 
        { 
            ApplicationStatus.Closed 
        },
        
        [ApplicationStatus.Declined] = new List<ApplicationStatus>(), // Terminal state
        [ApplicationStatus.Cancelled] = new List<ApplicationStatus>(), // Terminal state
        [ApplicationStatus.Closed] = new List<ApplicationStatus>() // Terminal state
    };
    
    public Task<bool> TransitionToStatusAsync(IRRRLApplication application, ApplicationStatus newStatus, string? userId = null, string? notes = null)
    {
        if (!CanTransitionTo(application.Status, newStatus))
        {
            return Task.FromResult(false);
        }
        
        var oldStatus = application.Status;
        application.Status = newStatus;
        
        // Create status history entry
        var historyEntry = new ApplicationStatusHistory
        {
            IRRRLApplicationId = application.Id,
            FromStatus = oldStatus,
            ToStatus = newStatus,
            ChangedByUserId = userId,
            Notes = notes,
            ChangedAt = DateTime.UtcNow
        };
        
        application.StatusHistory.Add(historyEntry);
        
        // Update important dates
        switch (newStatus)
        {
            case ApplicationStatus.Submitted:
                application.SubmittedDate = DateTime.UtcNow;
                break;
            case ApplicationStatus.Approved:
                application.ApprovalDate = DateTime.UtcNow;
                application.ApprovedBy = userId;
                break;
            case ApplicationStatus.Closed:
                application.CompletedDate = DateTime.UtcNow;
                application.ActualClosingDate = DateTime.UtcNow;
                break;
        }
        
        return Task.FromResult(true);
    }
    
    public bool CanTransitionTo(ApplicationStatus currentStatus, ApplicationStatus newStatus)
    {
        if (!_validTransitions.ContainsKey(currentStatus))
        {
            return false;
        }
        
        return _validTransitions[currentStatus].Contains(newStatus);
    }
    
    public List<ApplicationStatus> GetValidNextStatuses(ApplicationStatus currentStatus)
    {
        return _validTransitions.TryGetValue(currentStatus, out var statuses) 
            ? statuses 
            : new List<ApplicationStatus>();
    }
    
    public async Task StartAIAnalysisAsync(IRRRLApplication application)
    {
        // Check if cash-out application
        if (application.ApplicationType == ApplicationType.CashOut)
        {
            await TransitionToStatusAsync(application, ApplicationStatus.PendingApproval, "System", 
                "Cash-out application flagged for manual review");
        }
        else
        {
            await TransitionToStatusAsync(application, ApplicationStatus.AIAnalyzing, "System", 
                "Starting AI analysis for rate-and-term refinance");
        }
    }
    
    public async Task CompleteAIAnalysisAsync(IRRRLApplication application)
    {
        await TransitionToStatusAsync(application, ApplicationStatus.DocumentGathering, "System", 
            "AI analysis complete. Action items generated for loan officer.");
    }
    
    public async Task StartDocumentGatheringAsync(IRRRLApplication application)
    {
        await TransitionToStatusAsync(application, ApplicationStatus.DocumentGathering, "System", 
            "Document gathering phase started");
    }
    
    public async Task CompleteDocumentGatheringAsync(IRRRLApplication application)
    {
        await TransitionToStatusAsync(application, ApplicationStatus.AIProcessing, "System", 
            "All required documents received. Starting AI processing.");
    }
    
    public async Task PrepareForUnderwriterAsync(IRRRLApplication application)
    {
        await TransitionToStatusAsync(application, ApplicationStatus.FilePreparation, "System", 
            "Preparing final file package for underwriter");
        
        // After file preparation, automatically move to underwriter ready
        await Task.Delay(100); // Simulate some processing time
        
        await TransitionToStatusAsync(application, ApplicationStatus.UnderwriterReady, "System", 
            "File package complete and ready for underwriting");
    }
}

