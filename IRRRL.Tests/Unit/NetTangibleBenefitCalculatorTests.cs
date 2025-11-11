using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Core.Services;
using FluentAssertions;
using Xunit;

namespace IRRRL.Tests.Unit;

public class NetTangibleBenefitCalculatorTests
{
    private readonly NetTangibleBenefitCalculator _calculator;
    
    public NetTangibleBenefitCalculatorTests()
    {
        _calculator = new NetTangibleBenefitCalculator();
    }
    
    [Fact]
    public void CalculateMonthlyPayment_WithValidInputs_ReturnsCorrectAmount()
    {
        // Arrange
        decimal loanAmount = 200000;
        decimal annualInterestRate = 6.0m;
        int termMonths = 360;
        
        // Act
        var monthlyPayment = _calculator.CalculateMonthlyPayment(loanAmount, annualInterestRate, termMonths);
        
        // Assert
        monthlyPayment.Should().BeApproximately(1199.10m, 1.00m); // Within $1
    }
    
    [Fact]
    public void CalculateRecoupmentPeriod_WithValidInputs_ReturnsCorrectMonths()
    {
        // Arrange
        decimal totalLoanCosts = 6000;
        decimal monthlyPaymentSavings = 250;
        
        // Act
        var recoupmentPeriod = _calculator.CalculateRecoupmentPeriod(totalLoanCosts, monthlyPaymentSavings);
        
        // Assert
        recoupmentPeriod.Should().Be(24);
    }
    
    [Fact]
    public void CalculateRecoupmentPeriod_WithNoSavings_ReturnsMaxValue()
    {
        // Arrange
        decimal totalLoanCosts = 6000;
        decimal monthlyPaymentSavings = 0;
        
        // Act
        var recoupmentPeriod = _calculator.CalculateRecoupmentPeriod(totalLoanCosts, monthlyPaymentSavings);
        
        // Assert
        recoupmentPeriod.Should().Be(int.MaxValue);
    }
    
    [Fact]
    public void Calculate_WithValidApplication_PassesNTBTest()
    {
        // Arrange
        var application = CreateTestApplication();
        
        // Act
        var ntb = _calculator.Calculate(application);
        
        // Assert
        ntb.Should().NotBeNull();
        ntb.PassesNTBTest.Should().BeTrue();
        ntb.MeetsRecoupmentRequirement.Should().BeTrue();
        ntb.MeetsInterestRateRequirement.Should().BeTrue();
        ntb.MeetsPaymentReductionRequirement.Should().BeTrue();
        ntb.MonthlyPaymentSavings.Should().BeGreaterThan(0);
        ntb.RecoupmentPeriodMonths.Should().BeLessOrEqualTo(36);
    }
    
    [Fact]
    public void Calculate_WithInsufficientRateReduction_FailsNTBTest()
    {
        // Arrange
        var application = CreateTestApplication();
        application.CurrentLoan!.InterestRate = 6.0m;
        application.RequestedInterestRate = 5.75m; // Only 0.25% reduction
        
        // Act
        var ntb = _calculator.Calculate(application);
        
        // Assert
        ntb.MeetsInterestRateRequirement.Should().BeFalse();
        ntb.PassesNTBTest.Should().BeFalse();
    }
    
    [Fact]
    public void Calculate_WithLongRecoupmentPeriod_FailsNTBTest()
    {
        // Arrange
        var application = CreateTestApplication();
        application.TotalLoanCosts = 20000; // High costs
        
        // Act
        var ntb = _calculator.Calculate(application);
        
        // Assert
        ntb.RecoupmentPeriodMonths.Should().BeGreaterThan(36);
        ntb.MeetsRecoupmentRequirement.Should().BeFalse();
        ntb.PassesNTBTest.Should().BeFalse();
    }
    
    [Fact]
    public void Calculate_ARMToFixed_PassesWithoutRateReduction()
    {
        // Arrange
        var application = CreateTestApplication();
        application.CurrentLoan!.LoanType = LoanType.ARM;
        application.CurrentLoan.InterestRate = 5.5m;
        application.RequestedInterestRate = 5.5m; // Same rate
        application.NewLoanType = LoanType.FixedRate;
        
        // Act
        var ntb = _calculator.Calculate(application);
        
        // Assert
        ntb.MeetsInterestRateRequirement.Should().BeTrue(); // Stability benefit
        ntb.MeetsPaymentReductionRequirement.Should().BeTrue(); // ARM to fixed provides benefit
    }
    
    private IRRRLApplication CreateTestApplication()
    {
        return new IRRRLApplication
        {
            Id = 1,
            ApplicationType = ApplicationType.RateAndTerm,
            RequestedLoanAmount = 200000,
            RequestedInterestRate = 5.5m,
            RequestedTermMonths = 360,
            NewLoanType = LoanType.FixedRate,
            TotalLoanCosts = 6000,
            Borrower = new Borrower { Id = 1 },
            Property = new Property { Id = 1 },
            CurrentLoan = new CurrentLoan
            {
                Id = 1,
                LoanType = LoanType.FixedRate,
                CurrentBalance = 195000,
                InterestRate = 6.5m,
                RemainingTermMonths = 324,
                MonthlyPrincipalAndInterest = 1386,
                CurrentOnPayments = true,
                IsVALoan = true
            }
        };
    }
}

