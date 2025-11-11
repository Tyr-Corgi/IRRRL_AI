namespace IRRRL.Core.Entities;

/// <summary>
/// Veteran/borrower information
/// </summary>
public class Borrower : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string SSN { get; set; } = string.Empty; // Encrypted in database
    public DateTime DateOfBirth { get; set; }
    
    // VA-specific
    public string? VAFileNumber { get; set; }
    public bool HasDisabilityRating { get; set; }
    public int? DisabilityPercentage { get; set; }
    
    // Address
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    
    // Navigation properties
    public string UserId { get; set; } = string.Empty; // Link to Identity user
    public ICollection<IRRRLApplication> Applications { get; set; } = new List<IRRRLApplication>();
}

