using IRRRL.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IRRRL.Infrastructure.Data;

/// <summary>
/// Database initialization and seeding
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Ensure database is created
        await context.Database.MigrateAsync();
        
        // Seed roles
        await SeedRolesAsync(roleManager);
        
        // Seed default users
        await SeedAdminUserAsync(userManager);
        await SeedTestVeteranAsync(userManager);
        await SeedTestLoanOfficerAsync(userManager);
        
        // Seed test application data
        await SeedTestApplicationsAsync(context, userManager);
    }
    
    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = 
        {
            ApplicationConstants.Roles.Veteran,
            ApplicationConstants.Roles.LoanOfficer,
            ApplicationConstants.Roles.Underwriter,
            ApplicationConstants.Roles.Administrator
        };
        
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    
    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@irrrl.local";
        const string adminPassword = "Admin@123!"; // Should be changed in production
        
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true
            };
            
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, ApplicationConstants.Roles.Administrator);
            }
        }
    }
    
    private static async Task SeedTestVeteranAsync(UserManager<ApplicationUser> userManager)
    {
        const string veteranEmail = "veteran@irrrl.local";
        const string veteranPassword = "Veteran@123!"; // Should be changed in production
        
        var veteranUser = await userManager.FindByEmailAsync(veteranEmail);
        
        if (veteranUser == null)
        {
            veteranUser = new ApplicationUser
            {
                UserName = veteranEmail,
                Email = veteranEmail,
                EmailConfirmed = true,
                FirstName = "John",
                LastName = "Veteran",
                PhoneNumber = "(555) 123-4567",
                IsActive = true
            };
            
            var result = await userManager.CreateAsync(veteranUser, veteranPassword);
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(veteranUser, ApplicationConstants.Roles.Veteran);
            }
        }
    }
    
    private static async Task SeedTestLoanOfficerAsync(UserManager<ApplicationUser> userManager)
    {
        const string loanOfficerEmail = "loanofficer@irrrl.local";
        const string loanOfficerPassword = "LoanOfficer@123!";
        
        var loanOfficerUser = await userManager.FindByEmailAsync(loanOfficerEmail);
        
        if (loanOfficerUser == null)
        {
            loanOfficerUser = new ApplicationUser
            {
                UserName = loanOfficerEmail,
                Email = loanOfficerEmail,
                EmailConfirmed = true,
                FirstName = "Sarah",
                LastName = "Officer",
                PhoneNumber = "(555) 999-8888",
                IsActive = true
            };
            
            var result = await userManager.CreateAsync(loanOfficerUser, loanOfficerPassword);
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(loanOfficerUser, ApplicationConstants.Roles.LoanOfficer);
            }
        }
    }
    
    private static async Task SeedTestApplicationsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        // Check if we already have applications
        if (await context.IRRRLApplications.AnyAsync())
        {
            return; // Already seeded
        }
        
        var veteranUser = await userManager.FindByEmailAsync("veteran@irrrl.local");
        var loanOfficerUser = await userManager.FindByEmailAsync("loanofficer@irrrl.local");
        
        if (veteranUser == null || loanOfficerUser == null)
        {
            return; // Users not found
        }
        
        // Create test applications with the SeedTestApplicationData helper
        await SeedTestApplicationData(context, veteranUser.Id, loanOfficerUser.Id);
        
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedTestApplicationData(ApplicationDbContext context, string veteranUserId, string loanOfficerId)
    {
        using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            // Create Borrower
            var borrower = new Core.Entities.Borrower
            {
                FirstName = "John",
                LastName = "Smith",
                MiddleName = "M",
                Email = "john.smith@example.com",
                Phone = "(555) 123-4567",
                SSN = "123-45-6789", // Will be encrypted by the service
                DateOfBirth = new DateTime(1980, 5, 15),
                VAFileNumber = "123456789",
                HasDisabilityRating = true,
                DisabilityPercentage = 30,
                StreetAddress = "123 Main St",
                City = "San Diego",
                State = "CA",
                ZipCode = "92101",
                UserId = veteranUserId,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            };
            context.Borrowers.Add(borrower);
            await context.SaveChangesAsync();
            
                // Create Property
                var property = new Core.Entities.Property
                {
                    StreetAddress = "123 Main St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "92101",
                    PropertyType = "Single Family",
                    EstimatedValue = 400000,
                    YearBuilt = 2005,
                    CurrentlyOccupied = true,
                    PreviouslyOccupied = true,
                    OccupancyStartDate = DateTime.UtcNow.AddYears(-3),
                    AnnualPropertyTax = 4800,
                    AnnualInsurance = 1200,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                };
            context.Properties.Add(property);
            await context.SaveChangesAsync();
            
            // Create Application 1 - New
            var app1 = new Core.Entities.IRRRLApplication
            {
                ApplicationNumber = "IRRRL-2025-001",
                ApplicationType = Core.Enums.ApplicationType.RateAndTerm,
                Status = Core.Enums.ApplicationStatus.Submitted,
                BorrowerId = borrower.Id,
                PropertyId = property.Id,
                RequestedLoanAmount = 250000,
                RequestedInterestRate = 3.5m,
                RequestedTermMonths = 360,
                NewLoanType = Core.Enums.LoanType.FixedRate,
                EstimatedNewMonthlyPayment = 1123,
                EstimatedMonthlySavings = 377,
                RecoupmentPeriodMonths = 12,
                MeetsNetTangibleBenefit = true,
                FundingFeePercentage = 0.5m,
                FundingFeeAmount = 1250,
                FundingFeeWaived = false,
                TotalClosingCosts = 3000,
                TotalLoanCosts = 4250,
                EligibilityVerified = false,
                AssignedLoanOfficerId = loanOfficerId,
                SubmittedDate = DateTime.UtcNow.AddDays(-2),
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            };
            context.IRRRLApplications.Add(app1);
            await context.SaveChangesAsync();
            
                // Create Current Loan for App1
                var currentLoan1 = new Core.Entities.CurrentLoan
                {
                    IRRRLApplicationId = app1.Id,
                    LoanNumber = "12-3456789",
                    Lender = "ABC Mortgage Company",
                    CurrentBalance = 250000,
                    OriginalLoanAmount = 275000,
                    InterestRate = 4.5m,
                    LoanType = Core.Enums.LoanType.FixedRate,
                    OriginalTermMonths = 360,
                    RemainingTermMonths = 300,
                    MonthlyPrincipalAndInterest = 1394,
                    MonthlyPropertyTax = 400,
                    MonthlyInsurance = 100,
                    MonthlyPMI = 0,
                    TotalMonthlyPayment = 1894,
                    OriginationDate = DateTime.UtcNow.AddYears(-5),
                    CurrentOnPayments = true,
                    LatePaymentsLast12Months = 0,
                    LatePaymentsOver30Days = 0,
                    IsVALoan = true,
                    OriginalVAFundingFee = 2750,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                };
            context.CurrentLoans.Add(currentLoan1);
            
            // Add Action Items for App1
            var actionItems = new[]
            {
                new Core.Entities.ActionItem
                {
                    IRRRLApplicationId = app1.Id,
                    Title = "Verify Certificate of Eligibility",
                    Description = "Check if COE is current and reflects 30% disability rating for funding fee waiver consideration.",
                    Priority = Core.Enums.ActionItemPriority.High,
                    Status = Core.Enums.ActionItemStatus.Pending,
                    GeneratedByAI = true,
                    AIReasoning = "Borrower has 30% disability rating which may qualify for funding fee waiver",
                    DueDate = DateTime.UtcNow.AddDays(1),
                    OrderIndex = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Core.Entities.ActionItem
                {
                    IRRRLApplicationId = app1.Id,
                    Title = "Request Current Mortgage Statement",
                    Description = "Need most recent statement to verify exact payoff amount and loan number.",
                    Priority = Core.Enums.ActionItemPriority.High,
                    Status = Core.Enums.ActionItemStatus.Pending,
                    GeneratedByAI = true,
                    DueDate = DateTime.UtcNow.AddDays(2),
                    OrderIndex = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Core.Entities.ActionItem
                {
                    IRRRLApplicationId = app1.Id,
                    Title = "Calculate Net Tangible Benefit",
                    Description = "Verify NTB with proposed 3.5% rate. Ensure meets 0.5% reduction requirement.",
                    Priority = Core.Enums.ActionItemPriority.High,
                    Status = Core.Enums.ActionItemStatus.Completed,
                    CompletedDate = DateTime.UtcNow.AddDays(-1),
                    GeneratedByAI = true,
                    OrderIndex = 3,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };
            context.ActionItems.AddRange(actionItems);
            
            // Add Notes for App1
            var notes = new[]
            {
                new Core.Entities.ApplicationNote
                {
                    IRRRLApplicationId = app1.Id,
                    NoteType = Core.Enums.ApplicationNoteType.VeteranContact,
                    Content = "Initial call with John Smith to review application. Discussed timeline and next steps. Veteran expects closing in 30-45 days.",
                    IsImportant = false,
                    CreatedByUserId = loanOfficerId,
                    CreatedByName = "Sarah Officer",
                    CreatedAt = DateTime.UtcNow.AddHours(-3)
                },
                new Core.Entities.ApplicationNote
                {
                    IRRRLApplicationId = app1.Id,
                    NoteType = Core.Enums.ApplicationNoteType.Calculation,
                    Content = "Ran NTB calculator with 3.5% rate. Monthly savings: $377. Recoupment: 11 months. Meets all VA requirements.",
                    IsImportant = true,
                    CreatedByUserId = loanOfficerId,
                    CreatedByName = "Sarah Officer",
                    CreatedAt = DateTime.UtcNow.AddHours(-1)
                }
            };
            context.ApplicationNotes.AddRange(notes);
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

