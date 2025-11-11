namespace IRRRL.Core.Entities;

/// <summary>
/// Property information for the VA loan
/// </summary>
public class Property : BaseEntity
{
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string? County { get; set; }
    
    // Property details
    public string PropertyType { get; set; } = "Single Family"; // Single Family, Condo, Townhouse, etc.
    public int? YearBuilt { get; set; }
    public int? SquareFeet { get; set; }
    
    // Occupancy
    public bool CurrentlyOccupied { get; set; }
    public bool PreviouslyOccupied { get; set; }
    public DateTime? OccupancyStartDate { get; set; }
    public DateTime? OccupancyEndDate { get; set; }
    
    // Financial
    public decimal EstimatedValue { get; set; }
    public decimal AnnualPropertyTax { get; set; }
    public decimal AnnualInsurance { get; set; }
    
    // Navigation properties
    public ICollection<IRRRLApplication> Applications { get; set; } = new List<IRRRLApplication>();
}

