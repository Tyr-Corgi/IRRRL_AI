using IRRRL.Core.Entities;

namespace IRRRL.Core.Interfaces;

/// <summary>
/// Service for loan officer operations
/// </summary>
public interface ILoanOfficerService
{
    /// <summary>
    /// Get all applications for the loan officer dashboard
    /// </summary>
    Task<List<IRRRLApplication>> GetAllApplicationsAsync(string? loanOfficerId = null);
    
    /// <summary>
    /// Get a single application with all related data
    /// </summary>
    Task<IRRRLApplication?> GetApplicationDetailAsync(int applicationId);
    
    /// <summary>
    /// Update application status
    /// </summary>
    Task<bool> UpdateApplicationStatusAsync(int applicationId, string newStatus, string? note = null, string? userId = null);
    
    /// <summary>
    /// Add a note to an application
    /// </summary>
    Task<ApplicationNote> AddNoteAsync(int applicationId, string noteType, string content, bool isImportant, string userId, string userName);
    
    /// <summary>
    /// Get all notes for an application
    /// </summary>
    Task<List<ApplicationNote>> GetApplicationNotesAsync(int applicationId);
    
    /// <summary>
    /// Delete a note
    /// </summary>
    Task<bool> DeleteNoteAsync(int noteId, string userId);
    
    /// <summary>
    /// Toggle action item completion
    /// </summary>
    Task<bool> ToggleActionItemAsync(int actionItemId, string userId);
    
    /// <summary>
    /// Add a custom action item
    /// </summary>
    Task<ActionItem> AddActionItemAsync(int applicationId, string title, string description, string priority, string? userId = null);
    
    /// <summary>
    /// Delete an action item
    /// </summary>
    Task<bool> DeleteActionItemAsync(int actionItemId);
    
    /// <summary>
    /// Mark document as received
    /// </summary>
    Task<bool> ToggleDocumentReceivedAsync(int documentId);
    
    /// <summary>
    /// Get applications statistics
    /// </summary>
    Task<ApplicationStatistics> GetStatisticsAsync(string? loanOfficerId = null);
}

/// <summary>
/// Statistics for the dashboard
/// </summary>
public class ApplicationStatistics
{
    public int NewCount { get; set; }
    public int InProgressCount { get; set; }
    public int ReadyForReviewCount { get; set; }
    public int CompletedLast30DaysCount { get; set; }
}

