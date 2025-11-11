using IRRRL.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IRRRL.Infrastructure.Data;

/// <summary>
/// Main database context for IRRRL application
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    // Core entities
    public DbSet<Borrower> Borrowers => Set<Borrower>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<CurrentLoan> CurrentLoans => Set<CurrentLoan>();
    public DbSet<IRRRLApplication> IRRRLApplications => Set<IRRRLApplication>();
    public DbSet<NetTangibleBenefit> NetTangibleBenefits => Set<NetTangibleBenefit>();
    
    // Documents and actions
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<ActionItem> ActionItems => Set<ActionItem>();
    
    // Audit and history
    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories => Set<ApplicationStatusHistory>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure entity relationships and constraints
        ConfigureBorrower(builder);
        ConfigureProperty(builder);
        ConfigureCurrentLoan(builder);
        ConfigureIRRRLApplication(builder);
        ConfigureNetTangibleBenefit(builder);
        ConfigureDocument(builder);
        ConfigureActionItem(builder);
        ConfigureApplicationStatusHistory(builder);
        ConfigureAuditLog(builder);
    }
    
    private void ConfigureBorrower(ModelBuilder builder)
    {
        builder.Entity<Borrower>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            // SSN is encrypted, so needs to store much longer strings (encrypted data is ~200+ chars)
            entity.Property(e => e.SSN).IsRequired().HasMaxLength(500);
            entity.Property(e => e.UserId).IsRequired();
            
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.UserId).IsUnique();
        });
    }
    
    private void ConfigureProperty(ModelBuilder builder)
    {
        builder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StreetAddress).IsRequired().HasMaxLength(200);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).IsRequired().HasMaxLength(2);
            entity.Property(e => e.ZipCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.EstimatedValue).HasPrecision(18, 2);
            entity.Property(e => e.AnnualPropertyTax).HasPrecision(18, 2);
            entity.Property(e => e.AnnualInsurance).HasPrecision(18, 2);
        });
    }
    
    private void ConfigureCurrentLoan(ModelBuilder builder)
    {
        builder.Entity<CurrentLoan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LoanNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Lender).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CurrentBalance).HasPrecision(18, 2);
            entity.Property(e => e.OriginalLoanAmount).HasPrecision(18, 2);
            entity.Property(e => e.InterestRate).HasPrecision(5, 3);
            entity.Property(e => e.MonthlyPrincipalAndInterest).HasPrecision(18, 2);
            entity.Property(e => e.MonthlyPropertyTax).HasPrecision(18, 2);
            entity.Property(e => e.MonthlyInsurance).HasPrecision(18, 2);
            entity.Property(e => e.MonthlyPMI).HasPrecision(18, 2);
            entity.Property(e => e.TotalMonthlyPayment).HasPrecision(18, 2);
            entity.Property(e => e.OriginalVAFundingFee).HasPrecision(18, 2);
            
            entity.HasOne(e => e.IRRRLApplication)
                .WithOne(a => a.CurrentLoan)
                .HasForeignKey<CurrentLoan>(e => e.IRRRLApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
    
    private void ConfigureIRRRLApplication(ModelBuilder builder)
    {
        builder.Entity<IRRRLApplication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ApplicationNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RequestedLoanAmount).HasPrecision(18, 2);
            entity.Property(e => e.RequestedInterestRate).HasPrecision(5, 3);
            entity.Property(e => e.EstimatedNewMonthlyPayment).HasPrecision(18, 2);
            entity.Property(e => e.EstimatedMonthlySavings).HasPrecision(18, 2);
            entity.Property(e => e.FundingFeePercentage).HasPrecision(5, 3);
            entity.Property(e => e.FundingFeeAmount).HasPrecision(18, 2);
            entity.Property(e => e.TotalClosingCosts).HasPrecision(18, 2);
            entity.Property(e => e.TotalLoanCosts).HasPrecision(18, 2);
            entity.Property(e => e.CashOutAmount).HasPrecision(18, 2);
            
            entity.HasIndex(e => e.ApplicationNumber).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ApplicationType);
            
            entity.HasOne(e => e.Borrower)
                .WithMany(b => b.Applications)
                .HasForeignKey(e => e.BorrowerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Property)
                .WithMany(p => p.Applications)
                .HasForeignKey(e => e.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
    
    private void ConfigureNetTangibleBenefit(ModelBuilder builder)
    {
        builder.Entity<NetTangibleBenefit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CurrentInterestRate).HasPrecision(5, 3);
            entity.Property(e => e.CurrentMonthlyPayment).HasPrecision(18, 2);
            entity.Property(e => e.NewInterestRate).HasPrecision(5, 3);
            entity.Property(e => e.NewMonthlyPayment).HasPrecision(18, 2);
            entity.Property(e => e.InterestRateReduction).HasPrecision(5, 3);
            entity.Property(e => e.MonthlyPaymentSavings).HasPrecision(18, 2);
            entity.Property(e => e.TotalLoanCosts).HasPrecision(18, 2);
            entity.Property(e => e.TotalSavingsOverRemainingTerm).HasPrecision(18, 2);
            entity.Property(e => e.TotalInterestSavings).HasPrecision(18, 2);
            entity.Property(e => e.EquityGrowthAcceleration).HasPrecision(18, 2);
            entity.Property(e => e.BreakEvenMonths).HasPrecision(8, 2);
            
            entity.HasOne(e => e.IRRRLApplication)
                .WithOne(a => a.NetTangibleBenefitCalculation)
                .HasForeignKey<NetTangibleBenefit>(e => e.IRRRLApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
    
    private void ConfigureDocument(ModelBuilder builder)
    {
        builder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AIConfidenceScore).HasPrecision(3, 2);
            
            entity.HasIndex(e => e.DocumentType);
            entity.HasIndex(e => e.IsCurrentVersion);
            
            entity.HasOne(e => e.IRRRLApplication)
                .WithMany(a => a.Documents)
                .HasForeignKey(e => e.IRRRLApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
    
    private void ConfigureActionItem(ModelBuilder builder)
    {
        builder.Entity<ActionItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
            
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.AssignedToUserId);
            
            entity.HasOne(e => e.IRRRLApplication)
                .WithMany(a => a.ActionItems)
                .HasForeignKey(e => e.IRRRLApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
    
    private void ConfigureApplicationStatusHistory(ModelBuilder builder)
    {
        builder.Entity<ApplicationStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.IRRRLApplication)
                .WithMany(a => a.StatusHistory)
                .HasForeignKey(e => e.IRRRLApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.ChangedAt);
        });
    }
    
    private void ConfigureAuditLog(ModelBuilder builder)
    {
        builder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => e.IsAIAction);
            
            entity.HasOne(e => e.IRRRLApplication)
                .WithMany(a => a.AuditLogs)
                .HasForeignKey(e => e.IRRRLApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }
    
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }
    
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}

