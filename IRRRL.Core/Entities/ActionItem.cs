using IRRRL.Core.Enums;

namespace IRRRL.Core.Entities;

/// <summary>
/// AI-generated action item for loan officer
/// </summary>
public class ActionItem : BaseEntity
{
    public int IRRRLApplicationId { get; set; }
    public IRRRLApplication IRRRLApplication { get; set; } = null!;
    
    // Action item details
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActionItemPriority Priority { get; set; } = ActionItemPriority.Medium;
    public ActionItemStatus Status { get; set; } = ActionItemStatus.Pending;
    
    // Related to specific document type
    public DocumentType? RelatedDocumentType { get; set; }
    public int? RelatedDocumentId { get; set; }
    
    // Assignment
    public string? AssignedToUserId { get; set; }
    public DateTime? AssignedDate { get; set; }
    
    // Completion
    public DateTime? CompletedDate { get; set; }
    public string? CompletedByUserId { get; set; }
    public string? CompletionNotes { get; set; }
    
    // Ordering and dependencies
    public int OrderIndex { get; set; }
    public string? DependsOnActionItemIds { get; set; } // Comma-separated IDs
    
    // AI generation metadata
    public bool GeneratedByAI { get; set; } = true;
    public string? AIReasoning { get; set; }
    public DateTime? DueDate { get; set; }
    public int? EstimatedMinutes { get; set; }
}

