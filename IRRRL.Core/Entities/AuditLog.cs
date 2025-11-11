namespace IRRRL.Core.Entities;

/// <summary>
/// Comprehensive audit trail for all actions taken on an application
/// </summary>
public class AuditLog : BaseEntity
{
    public int? IRRRLApplicationId { get; set; }
    public IRRRLApplication? IRRRLApplication { get; set; }
    
    // What happened
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? Details { get; set; }
    
    // Who did it
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserRole { get; set; }
    
    // When and where
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    
    // For AI actions
    public bool IsAIAction { get; set; }
    public string? AIModel { get; set; }
    public string? AIPrompt { get; set; }
    public string? AIResponse { get; set; }
    
    // Change tracking
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
}

