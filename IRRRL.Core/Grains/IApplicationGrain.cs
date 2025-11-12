using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using Orleans;

namespace IRRRL.Core.Grains;

/// <summary>
/// Grain for managing a single IRRRL application
/// Each application gets its own Grain instance (distributed actor)
/// 
/// KEY CONCEPT: This Grain manages ONE application's state and operations
/// Orleans ensures thread-safe access (one message at a time)
/// State is automatically persisted and can be distributed across Silos
/// </summary>
public interface IApplicationGrain : IGrainWithIntegerKey
{
    /// <summary>
    /// Get the current application state
    /// </summary>
    Task<IRRRLApplication?> GetApplicationAsync();
    
    /// <summary>
    /// Update application status with audit trail
    /// This demonstrates Orleans handling state changes safely
    /// </summary>
    Task UpdateStatusAsync(ApplicationStatus newStatus, string? notes = null, string? changedByUserId = null);
    
    /// <summary>
    /// Add a note to the application
    /// Shows how Grains can encapsulate business logic
    /// </summary>
    Task AddNoteAsync(string content, ApplicationNoteType noteType, bool isImportant, string createdByUserId, string createdByName);
    
    /// <summary>
    /// Mark a document as received
    /// Demonstrates fine-grained operations on the Grain
    /// </summary>
    Task MarkDocumentReceivedAsync(int documentId);
    
    /// <summary>
    /// Complete an action item
    /// </summary>
    Task CompleteActionItemAsync(int actionItemId);
    
    /// <summary>
    /// Get application statistics (for dashboard)
    /// </summary>
    Task<ApplicationStatisticsDto> GetStatisticsAsync();
}

/// <summary>
/// DTO for application statistics
/// </summary>
public record ApplicationStatisticsDto(
    int DocumentsNeededCount,
    int OpenActionItemsCount,
    int NotesCount,
    ApplicationStatus CurrentStatus,
    DateTime LastUpdated
);

