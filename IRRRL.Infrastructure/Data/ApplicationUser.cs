using Microsoft.AspNetCore.Identity;

namespace IRRRL.Infrastructure.Data;

/// <summary>
/// Extended Identity user with additional properties
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Link to borrower profile if user is a veteran
    public int? BorrowerId { get; set; }
}

