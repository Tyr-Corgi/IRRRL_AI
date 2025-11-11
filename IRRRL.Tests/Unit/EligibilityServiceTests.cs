using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Core.Services;
using FluentAssertions;
using Xunit;

namespace IRRRL.Tests.Unit;

public class EligibilityServiceTests
{
    private readonly EligibilityService _service;
    
    public EligibilityServiceTests()
    {
        _service = new EligibilityService();
    }
    
    [Fact]
    public void HasExistingVALoan_WithVALoan_ReturnsTrue()
    {
        // Arrange
        var application = new IRRRLApplication
        {
            CurrentLoan = new CurrentLoan { IsVALoan = true }
        };
        
        // Act
        var result = _service.HasExistingVALoan(application);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void HasExistingVALoan_WithoutVALoan_ReturnsFalse()
    {
        // Arrange
        var application = new IRRRLApplication
        {
            CurrentLoan = new CurrentLoan { IsVALoan = false }
        };
        
        // Act
        var result = _service.HasExistingVALoan(application);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void IsCurrentOnPayments_WithCleanPaymentHistory_ReturnsTrue()
    {
        // Arrange
        var currentLoan = new CurrentLoan
        {
            CurrentOnPayments = true,
            LatePaymentsLast12Months = 0,
            LatePaymentsOver30Days = 0
        };
        
        // Act
        var result = _service.IsCurrentOnPayments(currentLoan);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsCurrentOnPayments_WithRecentLatePayment_ReturnsFalse()
    {
        // Arrange
        var currentLoan = new CurrentLoan
        {
            CurrentOnPayments = true,
            LatePaymentsLast12Months = 1,
            LatePaymentsOver30Days = 0,
            LastLatePaymentDate = DateTime.UtcNow.AddMonths(-2)
        };
        
        // Act
        var result = _service.IsCurrentOnPayments(currentLoan);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void MeetsOccupancyRequirements_WithCurrentOccupancy_ReturnsTrue()
    {
        // Arrange
        var property = new Property
        {
            CurrentlyOccupied = true,
            PreviouslyOccupied = false
        };
        
        // Act
        var result = _service.MeetsOccupancyRequirements(property);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void MeetsOccupancyRequirements_WithPreviousOccupancy_ReturnsTrue()
    {
        // Arrange
        var property = new Property
        {
            CurrentlyOccupied = false,
            PreviouslyOccupied = true
        };
        
        // Act
        var result = _service.MeetsOccupancyRequirements(property);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void MeetsOccupancyRequirements_WithNoOccupancy_ReturnsFalse()
    {
        // Arrange
        var property = new Property
        {
            CurrentlyOccupied = false,
            PreviouslyOccupied = false
        };
        
        // Act
        var result = _service.MeetsOccupancyRequirements(property);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void VerifyEligibility_WithEligibleApplication_ReturnsSuccess()
    {
        // Arrange
        var application = CreateEligibleApplication();
        
        // Act
        var result = _service.VerifyEligibility(application);
        
        // Assert
        result.IsEligible.Should().BeTrue();
        result.FailedChecks.Should().BeEmpty();
        result.PassedChecks.Should().NotBeEmpty();
    }
    
    [Fact]
    public void VerifyEligibility_WithoutVALoan_ReturnsFailure()
    {
        // Arrange
        var application = CreateEligibleApplication();
        application.CurrentLoan!.IsVALoan = false;
        
        // Act
        var result = _service.VerifyEligibility(application);
        
        // Assert
        result.IsEligible.Should().BeFalse();
        result.FailedChecks.Should().Contain(f => f.Contains("VA loan"));
    }
    
    private IRRRLApplication CreateEligibleApplication()
    {
        return new IRRRLApplication
        {
            Id = 1,
            ApplicationType = ApplicationType.RateAndTerm,
            Borrower = new Borrower
            {
                Id = 1,
                HasDisabilityRating = false
            },
            Property = new Property
            {
                Id = 1,
                CurrentlyOccupied = true,
                PreviouslyOccupied = true
            },
            CurrentLoan = new CurrentLoan
            {
                Id = 1,
                IsVALoan = true,
                CurrentOnPayments = true,
                LatePaymentsLast12Months = 0,
                LatePaymentsOver30Days = 0
            },
            NetTangibleBenefitCalculation = new NetTangibleBenefit
            {
                PassesNTBTest = true,
                MeetsRecoupmentRequirement = true,
                MeetsInterestRateRequirement = true,
                MeetsPaymentReductionRequirement = true,
                MonthlyPaymentSavings = 250,
                InterestRateReduction = 1.0m,
                RecoupmentPeriodMonths = 24
            }
        };
    }
}

