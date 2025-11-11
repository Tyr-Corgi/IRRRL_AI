using IRRRL.Core.Enums;

namespace IRRRL.Core.Entities;

/// <summary>
/// Document uploaded or generated for an application
/// </summary>
public class Document : BaseEntity
{
    public int IRRRLApplicationId { get; set; }
    public IRRRLApplication IRRRLApplication { get; set; } = null!;
    
    // Document details
    public DocumentType DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    
    // Document metadata
    public bool IsGenerated { get; set; } // true if AI/system generated, false if uploaded
    public DateTime? DocumentDate { get; set; } // Date on the document itself
    public DateTime? ExpirationDate { get; set; } // For time-sensitive documents
    public string? Description { get; set; }
    
    // Validation
    public bool IsValidated { get; set; }
    public DateTime? ValidatedDate { get; set; }
    public string? ValidatedBy { get; set; }
    public string? ValidationNotes { get; set; }
    
    // AI processing
    public bool AIProcessed { get; set; }
    public DateTime? AIProcessedDate { get; set; }
    public string? ExtractedData { get; set; } // JSON string of extracted data
    public decimal? AIConfidenceScore { get; set; }
    public string? AIProcessingNotes { get; set; }
    
    // Quality flags
    public bool IsComplete { get; set; }
    public bool IsLegible { get; set; }
    public bool IsCurrent { get; set; }
    public string? QualityIssues { get; set; }
    
    // Version control
    public int Version { get; set; } = 1;
    public int? PreviousVersionId { get; set; }
    public bool IsCurrentVersion { get; set; } = true;
}

