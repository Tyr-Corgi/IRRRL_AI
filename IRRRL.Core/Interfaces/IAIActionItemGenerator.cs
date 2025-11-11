using IRRRL.Core.Entities;
using IRRRL.Core.Enums;

namespace IRRRL.Core.Interfaces;

/// <summary>
/// AI service for generating action items for loan officers
/// </summary>
public interface IAIActionItemGenerator
{
    /// <summary>
    /// Generate initial action items after application submission
    /// </summary>
    Task<List<ActionItem>> GenerateInitialActionItemsAsync(IRRRLApplication application);
    
    /// <summary>
    /// Generate additional action items based on missing documents
    /// </summary>
    Task<List<ActionItem>> GenerateDocumentActionItemsAsync(IRRRLApplication application, List<DocumentType> missingDocuments);
    
    /// <summary>
    /// Generate follow-up action items after document review
    /// </summary>
    Task<List<ActionItem>> GenerateFollowUpActionItemsAsync(IRRRLApplication application, Document document, string? issues = null);
    
    /// <summary>
    /// Update action item priorities based on workflow progress
    /// </summary>
    Task UpdateActionItemPrioritiesAsync(IRRRLApplication application);
}

