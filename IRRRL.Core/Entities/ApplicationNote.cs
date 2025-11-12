using IRRRL.Core.Enums;

namespace IRRRL.Core.Entities;

/// <summary>
/// Internal note or communication log for an application
/// </summary>
public class ApplicationNote : BaseEntity
{
    public int IRRRLApplicationId { get; set; }
    public IRRRLApplication IRRRLApplication { get; set; } = null!;
    
    // Note details
    public ApplicationNoteType NoteType { get; set; } = ApplicationNoteType.General;
    public string Content { get; set; } = string.Empty;
    public bool IsImportant { get; set; }
    
    // Created by (loan officer, underwriter, etc.)
    public string CreatedByUserId { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty; // Denormalized for performance
    
    // Optional: Related document or action item
    public int? RelatedDocumentId { get; set; }
    public int? RelatedActionItemId { get; set; }
}

