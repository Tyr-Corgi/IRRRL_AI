using IRRRL.Core.Enums;

namespace IRRRL.Core.Entities;

/// <summary>
/// Tracks status changes throughout the application lifecycle
/// </summary>
public class ApplicationStatusHistory : BaseEntity
{
    public int IRRRLApplicationId { get; set; }
    public IRRRLApplication IRRRLApplication { get; set; } = null!;
    
    public ApplicationStatus FromStatus { get; set; }
    public ApplicationStatus ToStatus { get; set; }
    
    public string? ChangedByUserId { get; set; }
    public string? Notes { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}

