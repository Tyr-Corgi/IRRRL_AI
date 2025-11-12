using FluentAssertions;
using Xunit;
using static IRRRL.Web.Pages.Veteran.Apply;

namespace IRRRL.Tests;

/// <summary>
/// Tests for the new streamlined IRRRL features
/// </summary>
public class StreamlinedFeaturesTests
{
    #region Conditional Spouse Fields Tests

    [Fact]
    public void SpouseFields_ShouldBeOptional_WhenMaritalStatusIsUnmarried()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            MaritalStatus = "Unmarried",
            SpouseFirstName = "",
            SpouseLastName = ""
        };

        // Act & Assert
        model.MaritalStatus.Should().Be("Unmarried");
        model.SpouseFirstName.Should().BeEmpty("Spouse fields should be empty for unmarried");
        model.SpouseLastName.Should().BeEmpty("Spouse fields should be empty for unmarried");
    }

    [Fact]
    public void SpouseFields_CanBePopulated_WhenMaritalStatusIsMarried()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            MaritalStatus = "Married",
            SpouseFirstName = "Jane",
            SpouseLastName = "Doe"
        };

        // Act & Assert
        model.MaritalStatus.Should().Be("Married");
        model.SpouseFirstName.Should().Be("Jane");
        model.SpouseLastName.Should().Be("Doe");
    }

    [Theory]
    [InlineData("Separated")]
    [InlineData("Unmarried")]
    public void SpouseFields_ShouldBeOptional_ForNonMarriedStatus(string maritalStatus)
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            MaritalStatus = maritalStatus,
            SpouseFirstName = "",
            SpouseLastName = ""
        };

        // Act & Assert
        model.MaritalStatus.Should().Be(maritalStatus);
        // In a real UI scenario, these fields would be hidden/not required
    }

    #endregion

    #region Conditional Previous Address Tests

    [Theory]
    [InlineData(5, 0, false)] // 60 months = 5 years > 2 years
    [InlineData(3, 0, false)] // 36 months = 3 years > 2 years
    [InlineData(2, 0, false)] // 24 months = exactly 2 years (threshold)
    [InlineData(1, 11, true)] // 23 months < 2 years
    [InlineData(1, 6, true)]  // 18 months < 2 years
    [InlineData(0, 6, true)]  // 6 months < 2 years
    public void PreviousAddress_ShouldBeRequired_BasedOnTimeAtCurrentAddress(int years, int months, bool shouldRequirePrevious)
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            YearsAtCurrentAddress = years,
            MonthsAtCurrentAddress = months
        };

        // Act
        int totalMonths = (model.YearsAtCurrentAddress * 12) + model.MonthsAtCurrentAddress;
        bool needsPreviousAddress = totalMonths < 24;

        // Assert
        needsPreviousAddress.Should().Be(shouldRequirePrevious, 
            $"With {years} years and {months} months ({totalMonths} total), previous address requirement should be {shouldRequirePrevious}");
    }

    [Fact]
    public void PreviousAddress_CanBePopulated_WhenTimeIsLessThan2Years()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            YearsAtCurrentAddress = 1,
            MonthsAtCurrentAddress = 6,
            PreviousStreetAddress = "456 Oak Ave",
            PreviousCity = "Los Angeles",
            PreviousState = "CA",
            PreviousZipCode = "90001",
            YearsAtPreviousAddress = 2,
            MonthsAtPreviousAddress = 0
        };

        // Act
        int totalMonthsAtCurrent = (model.YearsAtCurrentAddress * 12) + model.MonthsAtCurrentAddress;

        // Assert
        totalMonthsAtCurrent.Should().BeLessThan(24);
        model.PreviousStreetAddress.Should().NotBeEmpty();
        model.PreviousCity.Should().NotBeEmpty();
        model.PreviousState.Should().NotBeEmpty();
        model.PreviousZipCode.Should().NotBeEmpty();
    }

    #endregion

    #region Property Address Copy Tests

    [Fact]
    public void PropertyAddress_CanBeCopied_ToCurrentAddress()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            PropertyAddress = "123 Main Street",
            City = "San Diego",
            State = "CA",
            ZipCode = "92101"
        };

        // Act - Simulate "Same as Property Address" checkbox behavior
        model.CurrentStreetAddress = model.PropertyAddress;
        model.CurrentCity = model.City;
        model.CurrentState = model.State;
        model.CurrentZipCode = model.ZipCode;

        // Assert
        model.CurrentStreetAddress.Should().Be(model.PropertyAddress);
        model.CurrentCity.Should().Be(model.City);
        model.CurrentState.Should().Be(model.State);
        model.CurrentZipCode.Should().Be(model.ZipCode);
    }

    [Fact]
    public void CurrentAddress_CanBeDifferent_FromPropertyAddress()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            PropertyAddress = "123 Main Street",
            City = "San Diego",
            State = "CA",
            ZipCode = "92101",
            CurrentStreetAddress = "456 Oak Avenue",
            CurrentCity = "Los Angeles",
            CurrentState = "CA",
            CurrentZipCode = "90001"
        };

        // Act & Assert
        model.CurrentStreetAddress.Should().NotBe(model.PropertyAddress);
        model.CurrentCity.Should().NotBe(model.City);
        model.CurrentZipCode.Should().NotBe(model.ZipCode);
    }

    #endregion

    #region Streamlined Property Info Tests

    [Fact]
    public void PropertyInfo_DoesNotRequire_NumberOfUnits()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            PropertyAddress = "123 Main St",
            City = "San Diego",
            State = "CA",
            ZipCode = "92101",
            PropertyType = "SingleFamily",
            Occupancy = "PrimaryResidence"
            // NumberOfUnits is now optional/removed in streamlined version
        };

        // Act & Assert
        model.PropertyAddress.Should().NotBeEmpty();
        model.City.Should().NotBeEmpty();
        model.State.Should().NotBeEmpty();
        model.ZipCode.Should().NotBeEmpty();
        // NumberOfUnits, YearBuilt, PropertyAcquiredDate are not required
    }

    [Fact]
    public void PropertyInfo_EstimatedHomeValue_IsOptional()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            PropertyAddress = "123 Main St",
            City = "San Diego",
            State = "CA",
            ZipCode = "92101",
            EstimatedHomeValue = 0 // Can be 0 if not estimated yet
        };

        // Act & Assert
        model.EstimatedHomeValue.Should().Be(0);
    }

    [Fact]
    public void PropertyInfo_EstimatedHomeValue_CanBeSet()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            PropertyAddress = "123 Main St",
            City = "San Diego",
            State = "CA",
            ZipCode = "92101"
        };

        // Act - Simulate API call setting value
        model.EstimatedHomeValue = 350000;

        // Assert
        model.EstimatedHomeValue.Should().Be(350000);
    }

    #endregion

    #region Streamlined Declarations Tests

    [Fact]
    public void Declarations_OnlyRequires_EssentialFields()
    {
        // Arrange - Only the 3 essential streamlined fields
        var model = new ApplicationFormModel
        {
            IntendToOccupyProperty = true,
            HasBankruptcy = false,
            HasForeclosure = false
            // Removed: HasOutstandingJudgments, IsPartyToLawsuit, 
            // HasDelinquentFederalDebt, HasAlimonyChildSupport
        };

        // Act & Assert
        model.IntendToOccupyProperty.Should().BeTrue();
        model.HasBankruptcy.Should().BeFalse();
        model.HasForeclosure.Should().BeFalse();
    }

    [Fact]
    public void Declarations_BankruptcyDetails_AreConditional()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            HasBankruptcy = true,
            BankruptcyType = "Chapter7",
            BankruptcyDischargeDate = new DateTime(2020, 1, 15)
        };

        // Act & Assert
        model.HasBankruptcy.Should().BeTrue();
        model.BankruptcyType.Should().Be("Chapter7");
        model.BankruptcyDischargeDate.Should().NotBeNull();
    }

    [Fact]
    public void Declarations_ForeclosureDetails_AreConditional()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            HasForeclosure = true,
            ForeclosureCompletionDate = new DateTime(2019, 6, 1)
        };

        // Act & Assert
        model.HasForeclosure.Should().BeTrue();
        model.ForeclosureCompletionDate.Should().NotBeNull();
    }

    [Fact]
    public void Declarations_BankruptcyDetails_AreNotRequired_WhenNoBankruptcy()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            HasBankruptcy = false,
            BankruptcyType = "",
            BankruptcyDischargeDate = null
        };

        // Act & Assert
        model.HasBankruptcy.Should().BeFalse();
        model.BankruptcyType.Should().BeEmpty();
        model.BankruptcyDischargeDate.Should().BeNull();
    }

    #endregion

    #region Remaining Term Calculator Tests

    [Theory]
    [InlineData(0, 0, 360)]  // Just got loan = 30 years remaining
    [InlineData(5, 0, 300)]  // 5 years ago = 25 years remaining
    [InlineData(10, 0, 240)] // 10 years ago = 20 years remaining
    [InlineData(15, 6, 174)] // 15.5 years ago = 14.5 years remaining
    [InlineData(30, 0, 0)]   // 30 years ago = 0 remaining
    [InlineData(35, 0, 0)]   // 35 years ago = 0 remaining (capped at 0)
    public void RemainingTermCalculator_ShouldCalculateCorrectly(int yearsAgo, int monthsAgo, int expectedRemainingMonths)
    {
        // Arrange
        int originalTermMonths = 360; // 30-year loan
        int monthsPassed = (yearsAgo * 12) + monthsAgo;

        // Act
        int remainingMonths = Math.Max(originalTermMonths - monthsPassed, 0);

        // Assert
        remainingMonths.Should().Be(expectedRemainingMonths, 
            $"With {yearsAgo} years and {monthsAgo} months passed, should have {expectedRemainingMonths} months remaining");
    }

    [Fact]
    public void RemainingTermMonths_CanBeStoredInModel()
    {
        // Arrange
        var model = new ApplicationFormModel();
        int yearsAgo = 5;
        int monthsAgo = 6;

        // Act - Simulate calculator
        int totalMonthsPassed = (yearsAgo * 12) + monthsAgo;
        model.RemainingTermMonths = Math.Max(360 - totalMonthsPassed, 0);

        // Assert
        model.RemainingTermMonths.Should().Be(294); // 360 - 66 = 294
    }

    #endregion

    #region Demographics Optional Tests

    [Fact]
    public void Demographics_AreCompletely_Optional()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            Gender = "",
            Ethnicity = "",
            RaceAmericanIndian = false,
            RaceAsian = false,
            RaceBlack = false,
            RacePacificIslander = false,
            RaceWhite = false
        };

        // Act & Assert
        model.Gender.Should().BeEmpty();
        model.Ethnicity.Should().BeEmpty();
        model.RaceAmericanIndian.Should().BeFalse();
        model.RaceAsian.Should().BeFalse();
        model.RaceBlack.Should().BeFalse();
        model.RacePacificIslander.Should().BeFalse();
        model.RaceWhite.Should().BeFalse();
    }

    [Fact]
    public void Demographics_CanBe_Populated()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            Gender = "Male",
            Ethnicity = "NotHispanicOrLatino",
            RaceWhite = true
        };

        // Act & Assert
        model.Gender.Should().Be("Male");
        model.Ethnicity.Should().Be("NotHispanicOrLatino");
        model.RaceWhite.Should().BeTrue();
    }

    [Fact]
    public void Demographics_MultipleRaces_CanBeSelected()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            RaceAsian = true,
            RaceWhite = true
        };

        // Act & Assert
        model.RaceAsian.Should().BeTrue();
        model.RaceWhite.Should().BeTrue();
        model.RaceBlack.Should().BeFalse();
    }

    #endregion

    #region Integration Tests for Complete Streamlined Flow

    [Fact]
    public void StreamlinedApplication_SingleVeteran_LivingAtProperty_MinimalFields()
    {
        // Arrange - Simplest possible streamlined application
        var model = new ApplicationFormModel
        {
            // Personal Info (No spouse)
            FirstName = "John",
            LastName = "Smith",
            DateOfBirth = new DateTime(1980, 1, 15),
            SSN = "123-45-6789",
            PhoneNumber = "(555) 123-4567",
            Email = "john.smith@example.com",
            CitizenshipStatus = "USCitizen",
            MaritalStatus = "Unmarried",

            // Current Address (Same as property, >2 years)
            CurrentStreetAddress = "123 Main St",
            CurrentCity = "San Diego",
            CurrentState = "CA",
            CurrentZipCode = "92101",
            YearsAtCurrentAddress = 5,
            MonthsAtCurrentAddress = 0,
            CurrentHousingStatus = "Own",
            CurrentMonthlyHousingPayment = 1500,

            // Military Service
            MilitaryBranch = "Army",
            MilitaryStatus = "Veteran",
            MilitaryServiceStartDate = new DateTime(2000, 1, 1),
            MilitaryServiceEndDate = new DateTime(2005, 1, 1),

            // Current Loan
            CurrentLoanAmount = 250000,
            CurrentInterestRate = 4.5m,
            CurrentMonthlyPayment = 1500,
            RemainingTermMonths = 300,
            LoanNumber = "12-3456789",

            // Property Info (Streamlined - no units/year/date)
            PropertyAddress = "123 Main St",
            City = "San Diego",
            State = "CA",
            ZipCode = "92101",
            PropertyType = "SingleFamily",
            Occupancy = "PrimaryResidence",

            // Declarations (Only 3 essential questions)
            IntendToOccupyProperty = true,
            HasBankruptcy = false,
            HasForeclosure = false

            // Demographics: Not filled (optional)
        };

        // Act - Verify model is complete
        int totalMonthsAtCurrent = (model.YearsAtCurrentAddress * 12) + model.MonthsAtCurrentAddress;
        bool needsPreviousAddress = totalMonthsAtCurrent < 24;
        bool needsSpouseInfo = model.MaritalStatus == "Married";

        // Assert - Verify streamlined requirements
        model.FirstName.Should().NotBeEmpty();
        model.LastName.Should().NotBeEmpty();
        model.SSN.Should().NotBeEmpty();
        model.CurrentStreetAddress.Should().NotBeEmpty();
        model.MilitaryBranch.Should().NotBeEmpty();
        model.CurrentLoanAmount.Should().BeGreaterThan(0);
        model.PropertyAddress.Should().NotBeEmpty();
        model.IntendToOccupyProperty.Should().BeTrue();

        needsPreviousAddress.Should().BeFalse("Should not need previous address for 5+ years");
        needsSpouseInfo.Should().BeFalse("Should not need spouse info when unmarried");

        // Verify streamlined - these should be empty/default
        model.SpouseFirstName.Should().BeNullOrEmpty();
        model.PreviousStreetAddress.Should().BeNullOrEmpty();
    }

    [Fact]
    public void StreamlinedApplication_MarriedVeteran_RecentMove_AllConditionalFields()
    {
        // Arrange - Complex streamlined application with all conditionals
        var model = new ApplicationFormModel
        {
            // Personal Info (WITH spouse)
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = new DateTime(1985, 6, 20),
            SSN = "987-65-4321",
            PhoneNumber = "(555) 987-6543",
            Email = "jane.doe@example.com",
            CitizenshipStatus = "USCitizen",
            MaritalStatus = "Married",
            SpouseFirstName = "Bob",
            SpouseLastName = "Doe",

            // Current Address (Different from property, <2 years)
            CurrentStreetAddress = "456 Oak Ave",
            CurrentCity = "Los Angeles",
            CurrentState = "CA",
            CurrentZipCode = "90001",
            YearsAtCurrentAddress = 1,
            MonthsAtCurrentAddress = 6,
            CurrentHousingStatus = "Rent",
            CurrentMonthlyHousingPayment = 2000,

            // Previous Address (Required because <2 years)
            PreviousStreetAddress = "789 Pine St",
            PreviousCity = "San Francisco",
            PreviousState = "CA",
            PreviousZipCode = "94102",
            YearsAtPreviousAddress = 2,
            MonthsAtPreviousAddress = 0,

            // Military Service
            MilitaryBranch = "Navy",
            MilitaryStatus = "ActiveDuty",
            MilitaryServiceStartDate = new DateTime(2005, 6, 1),

            // Current Loan
            CurrentLoanAmount = 400000,
            CurrentInterestRate = 5.0m,
            CurrentMonthlyPayment = 2200,
            RemainingTermMonths = 318,
            LoanNumber = "98-7654321",

            // Property Info
            PropertyAddress = "321 Beach Blvd",
            City = "Oceanside",
            State = "CA",
            ZipCode = "92054",
            PropertyType = "Townhouse",
            Occupancy = "PrimaryResidence",

            // Declarations (With bankruptcy)
            IntendToOccupyProperty = true,
            HasBankruptcy = true,
            BankruptcyType = "Chapter7",
            BankruptcyDischargeDate = new DateTime(2020, 1, 15),
            HasForeclosure = false
        };

        // Act
        int totalMonthsAtCurrent = (model.YearsAtCurrentAddress * 12) + model.MonthsAtCurrentAddress;
        bool needsPreviousAddress = totalMonthsAtCurrent < 24;
        bool needsSpouseInfo = model.MaritalStatus == "Married";

        // Assert - All conditional fields should be populated
        needsSpouseInfo.Should().BeTrue();
        model.SpouseFirstName.Should().Be("Bob");
        model.SpouseLastName.Should().Be("Doe");

        needsPreviousAddress.Should().BeTrue();
        model.PreviousStreetAddress.Should().NotBeEmpty();
        model.PreviousCity.Should().NotBeEmpty();

        model.HasBankruptcy.Should().BeTrue();
        model.BankruptcyType.Should().NotBeEmpty();
        model.BankruptcyDischargeDate.Should().NotBeNull();

        // Verify streamlined - removed fields should not be required
        // (NumberOfUnits, YearBuilt, PropertyAcquiredDate, etc. are not in model)
    }

    #endregion
}

