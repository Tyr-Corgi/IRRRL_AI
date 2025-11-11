using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace IRRRL.Infrastructure.Data;

/// <summary>
/// Seeds test data for development and testing
/// </summary>
public static class TestDataSeeder
{
    public static async Task SeedTestDataAsync(ApplicationDbContext context)
    {
        // Check if data already exists
        if (await context.IRRRLApplications.AnyAsync())
        {
            return; // Data already seeded
        }
        
        // Create test borrower
        var borrower = new Borrower
        {
            FirstName = "John",
            LastName = "Veteran",
            Email = "john.veteran@example.com",
            Phone = "(555) 123-4567",
            SSN = "123-45-6789", // Would be encrypted in production
            DateOfBirth = new DateTime(1980, 5, 15),
            VAFileNumber = "VA123456",
            HasDisabilityRating = false,
            DisabilityPercentage = 0,
            StreetAddress = "123 Main Street",
            City = "Springfield",
            State = "IL",
            ZipCode = "62701",
            UserId = "test-user-id"
        };
        
        context.Borrowers.Add(borrower);
        await context.SaveChangesAsync();
        
        // Create test property
        var property = new Property
        {
            StreetAddress = "123 Main Street",
            City = "Springfield",
            State = "IL",
            ZipCode = "62701",
            County = "Sangamon",
            PropertyType = "Single Family",
            YearBuilt = 2010,
            SquareFeet = 2000,
            CurrentlyOccupied = true,
            PreviouslyOccupied = true,
            OccupancyStartDate = new DateTime(2015, 3, 1),
            EstimatedValue = 250000,
            AnnualPropertyTax = 3500,
            AnnualInsurance = 1200
        };
        
        context.Properties.Add(property);
        await context.SaveChangesAsync();
        
        // Create test application
        var application = new IRRRLApplication
        {
            ApplicationNumber = "IRRRL-2025-0001",
            ApplicationType = ApplicationType.RateAndTerm,
            Status = ApplicationStatus.Submitted,
            BorrowerId = borrower.Id,
            PropertyId = property.Id,
            RequestedLoanAmount = 200000,
            RequestedInterestRate = 5.5m,
            RequestedTermMonths = 360,
            NewLoanType = LoanType.FixedRate,
            EstimatedNewMonthlyPayment = 1136,
            EstimatedMonthlySavings = 250,
            RecoupmentPeriodMonths = 24,
            MeetsNetTangibleBenefit = true,
            FundingFeePercentage = 0.5m,
            FundingFeeAmount = 1000,
            FundingFeeWaived = false,
            TotalClosingCosts = 5000,
            TotalLoanCosts = 6000,
            EligibilityVerified = false,
            SubmittedDate = DateTime.UtcNow,
            EstimatedClosingDate = DateTime.UtcNow.AddDays(45)
        };
        
        context.IRRRLApplications.Add(application);
        await context.SaveChangesAsync();
        
        // Create current loan
        var currentLoan = new CurrentLoan
        {
            IRRRLApplicationId = application.Id,
            LoanNumber = "VA-LOAN-123456",
            Lender = "Veterans United Home Loans",
            LoanType = LoanType.FixedRate,
            CurrentBalance = 195000,
            OriginalLoanAmount = 225000,
            InterestRate = 6.5m,
            RemainingTermMonths = 324,
            OriginalTermMonths = 360,
            OriginationDate = new DateTime(2020, 3, 1),
            MonthlyPrincipalAndInterest = 1386,
            MonthlyPropertyTax = 292,
            MonthlyInsurance = 100,
            MonthlyPMI = 0,
            TotalMonthlyPayment = 1778,
            CurrentOnPayments = true,
            LatePaymentsLast12Months = 0,
            LatePaymentsOver30Days = 0,
            IsVALoan = true,
            OriginalVAFundingFee = 5625
        };
        
        context.CurrentLoans.Add(currentLoan);
        await context.SaveChangesAsync();
        
        // Create NTB calculation
        var ntb = new NetTangibleBenefit
        {
            IRRRLApplicationId = application.Id,
            CurrentInterestRate = 6.5m,
            CurrentMonthlyPayment = 1386,
            CurrentRemainingTermMonths = 324,
            NewInterestRate = 5.5m,
            NewMonthlyPayment = 1136,
            NewTermMonths = 360,
            InterestRateReduction = 1.0m,
            MonthlyPaymentSavings = 250,
            TotalLoanCosts = 6000,
            RecoupmentPeriodMonths = 24,
            TotalSavingsOverRemainingTerm = 81000,
            TotalInterestSavings = 95000,
            TermReductionMonths = -36,
            EquityGrowthAcceleration = 15,
            MeetsRecoupmentRequirement = true,
            MeetsInterestRateRequirement = true,
            MeetsPaymentReductionRequirement = true,
            PassesNTBTest = true,
            BreakEvenMonths = 24,
            CalculatedDate = DateTime.UtcNow,
            CalculatedBy = "System"
        };
        
        context.NetTangibleBenefits.Add(ntb);
        
        // Create initial action items
        var actionItems = new List<ActionItem>
        {
            new ActionItem
            {
                IRRRLApplicationId = application.Id,
                Title = "Collect Current VA Loan Statement",
                Description = "Most recent mortgage statement showing loan number, current balance, interest rate, and monthly payment.",
                Priority = ActionItemPriority.Critical,
                Status = ActionItemStatus.Pending,
                RelatedDocumentType = DocumentType.VALoanStatement,
                OrderIndex = 1,
                GeneratedByAI = true,
                AIReasoning = "Required document for RateAndTerm IRRRL application",
                EstimatedMinutes = 10,
                DueDate = DateTime.UtcNow.AddDays(3)
            },
            new ActionItem
            {
                IRRRLApplicationId = application.Id,
                Title = "Collect Certificate of Eligibility (COE)",
                Description = "Can be obtained electronically through the VA or from veteran's records.",
                Priority = ActionItemPriority.Critical,
                Status = ActionItemStatus.Pending,
                RelatedDocumentType = DocumentType.CertificateOfEligibility,
                OrderIndex = 2,
                GeneratedByAI = true,
                AIReasoning = "Required document for RateAndTerm IRRRL application",
                EstimatedMinutes = 15,
                DueDate = DateTime.UtcNow.AddDays(3)
            },
            new ActionItem
            {
                IRRRLApplicationId = application.Id,
                Title = "Collect Photo ID",
                Description = "Driver's license or government-issued photo identification.",
                Priority = ActionItemPriority.High,
                Status = ActionItemStatus.Pending,
                RelatedDocumentType = DocumentType.PhotoID,
                OrderIndex = 3,
                GeneratedByAI = true,
                AIReasoning = "Required document for RateAndTerm IRRRL application",
                EstimatedMinutes = 5,
                DueDate = DateTime.UtcNow.AddDays(5)
            }
        };
        
        context.ActionItems.AddRange(actionItems);
        
        // Create status history
        var statusHistory = new ApplicationStatusHistory
        {
            IRRRLApplicationId = application.Id,
            FromStatus = ApplicationStatus.Submitted,
            ToStatus = ApplicationStatus.Submitted,
            ChangedByUserId = borrower.UserId,
            Notes = "Application submitted by veteran",
            ChangedAt = DateTime.UtcNow
        };
        
        context.ApplicationStatusHistories.Add(statusHistory);
        
        await context.SaveChangesAsync();
    }
}

