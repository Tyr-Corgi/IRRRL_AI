using IRRRL.Core.Entities;

namespace IRRRL.Core.Interfaces;

/// <summary>
/// AI service for document validation and quality checking
/// </summary>
public interface IAIDocumentValidator
{
    /// <summary>
    /// Validate document quality and completeness
    /// </summary>
    Task<DocumentValidationResult> ValidateDocumentAsync(Document document, byte[] fileContent);
    
    /// <summary>
    /// Extract data from document using OCR
    /// </summary>
    Task<Dictionary<string, string>> ExtractDataAsync(Document document, byte[] fileContent);
    
    /// <summary>
    /// Verify document matches expected type
    /// </summary>
    Task<bool> VerifyDocumentTypeAsync(Document document, byte[] fileContent);
}

/// <summary>
/// Result of document validation
/// </summary>
public class DocumentValidationResult
{
    public bool IsValid { get; set; }
    public bool IsLegible { get; set; } = true;
    public bool IsComplete { get; set; } = true;
    public bool IsCurrent { get; set; } = true;
    public decimal ConfidenceScore { get; set; }
    public List<string> Issues { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public Dictionary<string, string> ExtractedData { get; set; } = new();
    public string? Notes { get; set; }
}

