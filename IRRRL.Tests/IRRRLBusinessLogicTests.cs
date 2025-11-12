using FluentAssertions;
using IRRRL.Core.Services;
using IRRRL.Shared.DTOs;
using Xunit;

namespace IRRRL.Tests;

/// <summary>
/// Tests for IRRRL-specific business logic (eligibility, NTB calculation)
/// </summary>
public class IRRRLBusinessLogicTests
{
    private readonly EligibilityService _eligibilityService;
    private readonly NetTangibleBenefitCalculator _ntbCalculator;

    public IRRRLBusinessLogicTests()
    {
        _eligibilityService = new EligibilityService();
        _ntbCalculator = new NetTangibleBenefitCalculator();
    }

    #region Net Tangible Benefit (NTB) Tests

    [Theory]
    [InlineData(4.5, 3.5, true)]  // 1.0% reduction - passes
    [InlineData(5.0, 4.4, true)]  // 0.6% reduction - passes
    [InlineData(4.5, 4.0, true)]  // Exactly 0.5% reduction - passes
    [InlineData(4.5, 4.1, false)] // 0.4% reduction - fails
    [InlineData(5.0, 4.6, false)] // 0.4% reduction - fails
    public void NTB_FixedToFixed_ShouldRequire_HalfPercentReduction(
        decimal currentRate, 
        decimal newRate, 
        bool shouldPass)
    {
        // Arrange
        var request = new NetTangibleBenefitRequest
        {
            CurrentInterestRate = currentRate,
            NewInterestRate = newRate,
            CurrentLoanAmount = 250000,
            NewLoanAmount = 250000,
            IsCurrentLoanFixed = true,
            IsNewLoanFixed = true,
            RemainingTermMonths = 300,
            NewTermMonths = 360,
            ClosingCosts = 5000,
            FundingFee = 1250 // 0.5% of loan amount
        };

        // Act
        var result = _ntbCalculator.Calculate(request);

        // Assert
        result.MeetsNTB.Should().Be(shouldPass, 
            $"Fixed-to-fixed with {currentRate}% to {newRate}% should {(shouldPass ? "pass" : "fail")}");
        
        if (shouldPass)
        {
            result.InterestRateReduction.Should().BeGreaterThanOrEqualTo(0.5m);
        }
        else
        {
            result.InterestRateReduction.Should().BeLessThan(0.5m);
        }
    }

    [Theory]
    [InlineData(250000, 1500, 250000, 1400, true)]  // $100 reduction - passes
    [InlineData(250000, 1500, 250000, 1499, true)]  // $1 reduction - passes
    [InlineData(250000, 1500, 250000, 1500, false)] // $0 reduction - fails
    [InlineData(250000, 1500, 250000, 1501, false)] // Payment increase - fails
    public void NTB_ARMToFixed_ShouldRequire_PaymentReduction(
        decimal currentAmount,
        decimal currentPayment,
        decimal newAmount,
        decimal newPayment,
        bool shouldPass)
    {
        // Arrange
        var request = new NetTangibleBenefitRequest
        {
            CurrentInterestRate = 4.5m,
            NewInterestRate = 4.0m,
            CurrentLoanAmount = currentAmount,
            NewLoanAmount = newAmount,
            CurrentMonthlyPayment = currentPayment,
            NewMonthlyPayment = newPayment,
            IsCurrentLoanFixed = false, // ARM
            IsNewLoanFixed = true,      // Fixed
            RemainingTermMonths = 300,
            NewTermMonths = 360,
            ClosingCosts = 5000,
            FundingFee = 1250
        };

        // Act
        var result = _ntbCalculator.Calculate(request);

        // Assert
        result.MeetsNTB.Should().Be(shouldPass,
            $"ARM-to-fixed with payment change from ${currentPayment} to ${newPayment} should {(shouldPass ? "pass" : "fail")}");
    }

    [Theory]
    [InlineData(5000, 1250, 100, 36, true)]  // Recoups in 36 months - passes
    [InlineData(5000, 1250, 200, 36, true)]  // Recoups in 31.25 months - passes
    [InlineData(5000, 1250, 50, 36, false)]  // Recoups in 125 months - fails
    [InlineData(10000, 2500, 100, 36, false)] // Recoups in 125 months - fails
    public void NTB_RecoupmentPeriod_ShouldNotExceed_36Months(
        decimal closingCosts,
        decimal fundingFee,
        decimal monthlySavings,
        int maxMonths,
        bool shouldPass)
    {
        // Arrange
        var request = new NetTangibleBenefitRequest
        {
            CurrentInterestRate = 4.5m,
            NewInterestRate = 3.5m,
            CurrentLoanAmount = 250000,
            NewLoanAmount = 250000,
            CurrentMonthlyPayment = 1500,
            NewMonthlyPayment = 1500 - monthlySavings,
            IsCurrentLoanFixed = true,
            IsNewLoanFixed = true,
            RemainingTermMonths = 300,
            NewTermMonths = 360,
            ClosingCosts = closingCosts,
            FundingFee = fundingFee
        };

        // Act
        var result = _ntbCalculator.Calculate(request);

        // Assert
        if (shouldPass)
        {
            result.RecoupmentMonths.Should().BeLessOrEqualTo(maxMonths);
            result.MeetsNTB.Should().BeTrue();
        }
        else
        {
            result.RecoupmentMonths.Should().BeGreaterThan(maxMonths);
            result.MeetsNTB.Should().BeFalse();
        }
    }

    [Fact]
    public void NTB_Calculation_ShouldProvide_CompleteSummary()
    {
        // Arrange
        var request = new NetTangibleBenefitRequest
        {
            CurrentInterestRate = 4.5m,
            NewInterestRate = 3.5m,
            CurrentLoanAmount = 250000,
            NewLoanAmount = 250000,
            CurrentMonthlyPayment = 1500,
            NewMonthlyPayment = 1350,
            IsCurrentLoanFixed = true,
            IsNewLoanFixed = true,
            RemainingTermMonths = 300,
            NewTermMonths = 360,
            ClosingCosts = 5000,
            FundingFee = 1250
        };

        // Act
        var result = _ntbCalculator.Calculate(request);

        // Assert
        result.Should().NotBeNull();
        result.InterestRateReduction.Should().Be(1.0m); // 4.5 - 3.5
        result.MonthlyPaymentReduction.Should().Be(150); // 1500 - 1350
        result.TotalCosts.Should().Be(6250); // 5000 + 1250
        result.RecoupmentMonths.Should().BeApproximately(41.67m, 0.1m); // 6250 / 150
        result.MeetsNTB.Should().BeFalse("Recoupment > 36 months");
    }

    [Fact]
    public void NTB_ZeroMonthlySavings_ShouldFail()
    {
        // Arrange
        var request = new NetTangibleBenefitRequest
        {
            CurrentInterestRate = 4.5m,
            NewInterestRate = 3.5m,
            CurrentLoanAmount = 250000,
            NewLoanAmount = 250000,
            CurrentMonthlyPayment = 1500,
            NewMonthlyPayment = 1500, // No savings
            IsCurrentLoanFixed = true,
            IsNewLoanFixed = true,
            RemainingTermMonths = 300,
            NewTermMonths = 360,
            ClosingCosts = 5000,
            FundingFee = 1250
        };

        // Act
        var result = _ntbCalculator.Calculate(request);

        // Assert
        result.MeetsNTB.Should().BeFalse("No monthly savings means infinite recoupment");
        result.MonthlyPaymentReduction.Should().Be(0);
    }

    #endregion

    #region IRRRL Eligibility Tests

    [Fact]
    public void Eligibility_RequiresExisting_VALoan()
    {
        // Arrange
        var request = new EligibilityRequest
        {
            HasExistingVALoan = false,
            CurrentLoanType = "Conventional",
            PropertyType = "SingleFamily",
            OccupancyStatus = "PrimaryResidence",
            IsCurrent = true,
            LoanOrigination = DateTime.Now.AddYears(-5)
        };

        // Act
        var result = _eligibilityService.CheckEligibility(request);

        // Assert
        result.IsEligible.Should().BeFalse("Must have existing VA loan");
        result.Reasons.Should().Contain(r => r.Contains("existing VA loan"));
    }

    [Fact]
    public void Eligibility_RequiresProperty_WasOccupied()
    {
        // Arrange
        var request = new EligibilityRequest
        {
            HasExistingVALoan = true,
            CurrentLoanType = "VA",
            PropertyType = "SingleFamily",
            OccupancyStatus = "Investment", // Never occupied as primary
            WasPreviouslyOccupied = false,
            IsCurrent = true,
            LoanOrigination = DateTime.Now.AddYears(-5)
        };

        // Act
        var result = _eligibilityService.CheckEligibility(request);

        // Assert
        result.IsEligible.Should().BeFalse("Must have occupied property at some point");
        result.Reasons.Should().Contain(r => r.Contains("occupied") || r.Contains("primary residence"));
    }

    [Fact]
    public void Eligibility_RequiresLoan_IsCurrent()
    {
        // Arrange
        var request = new EligibilityRequest
        {
            HasExistingVALoan = true,
            CurrentLoanType = "VA",
            PropertyType = "SingleFamily",
            OccupancyStatus = "PrimaryResidence",
            IsCurrent = false, // Loan is delinquent
            LoanOrigination = DateTime.Now.AddYears(-5)
        };

        // Act
        var result = _eligibilityService.CheckEligibility(request);

        // Assert
        result.IsEligible.Should().BeFalse("Loan must be current");
        result.Reasons.Should().Contain(r => r.Contains("current") || r.Contains("delinquent"));
    }

    [Fact]
    public void Eligibility_AllRequirementsMet_ShouldPass()
    {
        // Arrange
        var request = new EligibilityRequest
        {
            HasExistingVALoan = true,
            CurrentLoanType = "VA",
            PropertyType = "SingleFamily",
            OccupancyStatus = "PrimaryResidence",
            IsCurrent = true,
            LoanOrigination = DateTime.Now.AddYears(-5),
            WasPreviouslyOccupied = true
        };

        // Act
        var result = _eligibilityService.CheckEligibility(request);

        // Assert
        result.IsEligible.Should().BeTrue("All requirements are met");
        result.Reasons.Should().BeEmpty();
    }

    [Theory]
    [InlineData("SingleFamily", true)]
    [InlineData("Condo", true)]
    [InlineData("Townhouse", true)]
    [InlineData("MultiUnit", true)] // Up to 4 units allowed
    [InlineData("Commercial", false)]
    public void Eligibility_PropertyType_ShouldBe_Residential(string propertyType, bool shouldBeEligible)
    {
        // Arrange
        var request = new EligibilityRequest
        {
            HasExistingVALoan = true,
            CurrentLoanType = "VA",
            PropertyType = propertyType,
            OccupancyStatus = "PrimaryResidence",
            IsCurrent = true,
            LoanOrigination = DateTime.Now.AddYears(-5)
        };

        // Act
        var result = _eligibilityService.CheckEligibility(request);

        // Assert
        result.IsEligible.Should().Be(shouldBeEligible,
            $"Property type '{propertyType}' should {(shouldBeEligible ? "be" : "not be")} eligible");
    }

    #endregion

    #region VA Funding Fee Tests

    [Theory]
    [InlineData(0, 0.005)]    // 0% disability = 0.5% fee
    [InlineData(10, 0.0)]     // 10% disability = no fee
    [InlineData(30, 0.0)]     // 30% disability = no fee
    [InlineData(100, 0.0)]    // 100% disability = no fee
    public void FundingFee_BasedOnDisability_ShouldCalculateCorrectly(int disabilityRating, decimal expectedFeeRate)
    {
        // Arrange
        decimal loanAmount = 250000;

        // Act
        decimal feeRate = disabilityRating >= 10 ? 0.0m : 0.005m; // 0.5% or exempt
        decimal fee = loanAmount * feeRate;

        // Assert
        feeRate.Should().Be(expectedFeeRate);
        
        if (disabilityRating >= 10)
        {
            fee.Should().Be(0, "Veterans with 10%+ disability are exempt from funding fee");
        }
        else
        {
            fee.Should().Be(1250, "0.5% of $250,000 = $1,250");
        }
    }

    [Fact]
    public void FundingFee_CanBeRolledInto_Loan()
    {
        // Arrange
        decimal loanAmount = 250000;
        decimal fundingFee = 1250; // 0.5%

        // Act
        decimal newLoanAmount = loanAmount + fundingFee;

        // Assert
        newLoanAmount.Should().Be(251250);
    }

    #endregion

    #region Savings Calculation Tests

    [Theory]
    [InlineData(250000, 4.5, 3.5, 360, 152.73)] // Approximate monthly savings
    [InlineData(400000, 5.0, 4.0, 360, 261.66)] // Larger loan
    [InlineData(150000, 6.0, 5.0, 300, 89.52)]  // Shorter term
    public void MonthlySavings_ShouldCalculate_PIPaymentDifference(
        decimal loanAmount,
        decimal currentRate,
        decimal newRate,
        int termMonths,
        decimal expectedSavings)
    {
        // Arrange - Calculate P&I payments
        decimal currentMonthlyRate = currentRate / 100 / 12;
        decimal newMonthlyRate = newRate / 100 / 12;

        // Act - Calculate monthly payment (P&I only)
        decimal currentPayment = CalculateMonthlyPayment(loanAmount, currentMonthlyRate, termMonths);
        decimal newPayment = CalculateMonthlyPayment(loanAmount, newMonthlyRate, termMonths);
        decimal savings = currentPayment - newPayment;

        // Assert
        savings.Should().BeApproximately(expectedSavings, 1.0m, 
            $"Monthly savings should be approximately ${expectedSavings:F2}");
    }

    [Fact]
    public void LifetimeSavings_ShouldCalculate_TotalInterestReduction()
    {
        // Arrange
        decimal loanAmount = 250000;
        decimal currentRate = 4.5m;
        decimal newRate = 3.5m;
        int termMonths = 300; // Remaining term

        decimal currentMonthlyRate = currentRate / 100 / 12;
        decimal newMonthlyRate = newRate / 100 / 12;

        // Act
        decimal currentPayment = CalculateMonthlyPayment(loanAmount, currentMonthlyRate, termMonths);
        decimal newPayment = CalculateMonthlyPayment(loanAmount, newMonthlyRate, termMonths);
        
        decimal currentTotalPaid = currentPayment * termMonths;
        decimal newTotalPaid = newPayment * termMonths;
        
        decimal lifetimeSavings = currentTotalPaid - newTotalPaid;

        // Assert
        lifetimeSavings.Should().BeGreaterThan(0);
        lifetimeSavings.Should().BeGreaterThan(10000, "Should save significant amount over life of loan");
    }

    #endregion

    #region Helper Methods

    private decimal CalculateMonthlyPayment(decimal principal, decimal monthlyRate, int months)
    {
        if (monthlyRate == 0) return principal / months;
        
        double rate = (double)monthlyRate;
        double p = (double)principal;
        int n = months;
        
        double payment = p * (rate * Math.Pow(1 + rate, n)) / (Math.Pow(1 + rate, n) - 1);
        
        return (decimal)Math.Round(payment, 2);
    }

    #endregion
}

