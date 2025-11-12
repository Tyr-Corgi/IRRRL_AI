using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;
using static IRRRL.Web.Pages.Veteran.Apply;

namespace IRRRL.Tests;

/// <summary>
/// Tests for the ApplicationFormModel validation and business logic
/// </summary>
public class ApplicationFormModelTests
{
    [Fact]
    public void PersonalInfo_RequiredFields_ShouldFailValidation_WhenEmpty()
    {
        // Arrange
        var model = new ApplicationFormModel
        {
            FirstName = "",
            LastName = "",
            SSN = "",
            PhoneNumber = "",
            Email = "",
            CitizenshipStatus = "",
            MaritalStatus = ""
        };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.FirstName)));
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.LastName)));
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.SSN)));
    }

    [Theory]
    [InlineData("123-45-6789", true)]
    [InlineData("987-65-4321", true)]
    [InlineData("123456789", false)]
    [InlineData("123-45-678", false)]
    [InlineData("abc-de-fghi", false)]
    public void SSN_Validation_ShouldMatchExpectedFormat(string ssn, bool shouldBeValid)
    {
        // Arrange
        var model = CreateValidModel();
        model.SSN = ssn;

        // Act
        var validationResults = ValidateModel(model);
        var hasSSNError = validationResults.Any(v => v.MemberNames.Contains(nameof(ApplicationFormModel.SSN)));

        // Assert
        if (shouldBeValid)
        {
            hasSSNError.Should().BeFalse($"SSN '{ssn}' should be valid");
        }
        else
        {
            hasSSNError.Should().BeTrue($"SSN '{ssn}' should be invalid");
        }
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name@domain.co.uk", true)]
    [InlineData("invalid-email", false)]
    [InlineData("@example.com", false)]
    [InlineData("test@", false)]
    public void Email_Validation_ShouldMatchExpectedFormat(string email, bool shouldBeValid)
    {
        // Arrange
        var model = CreateValidModel();
        model.Email = email;

        // Act
        var validationResults = ValidateModel(model);
        var hasEmailError = validationResults.Any(v => v.MemberNames.Contains(nameof(ApplicationFormModel.Email)));

        // Assert
        if (shouldBeValid)
        {
            hasEmailError.Should().BeFalse($"Email '{email}' should be valid");
        }
        else
        {
            hasEmailError.Should().BeTrue($"Email '{email}' should be invalid");
        }
    }

    [Theory]
    [InlineData("92101", true)]
    [InlineData("92101-1234", true)]
    [InlineData("12345", true)]
    [InlineData("1234", false)]
    [InlineData("123456", false)]
    [InlineData("abcde", false)]
    public void ZipCode_Validation_ShouldMatchExpectedFormat(string zipCode, bool shouldBeValid)
    {
        // Arrange
        var model = CreateValidModel();
        model.ZipCode = zipCode;
        model.CurrentZipCode = zipCode;

        // Act
        var validationResults = ValidateModel(model);
        var hasZipError = validationResults.Any(v => 
            v.MemberNames.Contains(nameof(ApplicationFormModel.ZipCode)) ||
            v.MemberNames.Contains(nameof(ApplicationFormModel.CurrentZipCode)));

        // Assert
        if (shouldBeValid)
        {
            hasZipError.Should().BeFalse($"ZIP '{zipCode}' should be valid");
        }
        else
        {
            hasZipError.Should().BeTrue($"ZIP '{zipCode}' should be invalid");
        }
    }

    [Theory]
    [InlineData(10000, 15.0, true)]
    [InlineData(250000, 4.5, true)]
    [InlineData(2000000, 1.0, true)]
    [InlineData(5000, 4.5, false)] // Below minimum
    [InlineData(3000000, 4.5, false)] // Above maximum
    [InlineData(250000, 0.5, false)] // Interest rate too low
    [InlineData(250000, 20.0, false)] // Interest rate too high
    public void LoanAmount_And_InterestRate_ShouldValidateRanges(decimal loanAmount, decimal interestRate, bool shouldBeValid)
    {
        // Arrange
        var model = CreateValidModel();
        model.CurrentLoanAmount = loanAmount;
        model.CurrentInterestRate = interestRate;

        // Act
        var validationResults = ValidateModel(model);
        var hasLoanError = validationResults.Any(v => 
            v.MemberNames.Contains(nameof(ApplicationFormModel.CurrentLoanAmount)) ||
            v.MemberNames.Contains(nameof(ApplicationFormModel.CurrentInterestRate)));

        // Assert
        if (shouldBeValid)
        {
            hasLoanError.Should().BeFalse($"Loan amount {loanAmount} and rate {interestRate}% should be valid");
        }
        else
        {
            hasLoanError.Should().BeTrue($"Loan amount {loanAmount} and rate {interestRate}% should be invalid");
        }
    }

    [Fact]
    public void SpouseFields_AreOptional_WhenNotMarried()
    {
        // Arrange
        var model = CreateValidModel();
        model.MaritalStatus = "Unmarried";
        model.SpouseFirstName = ""; // Empty spouse fields
        model.SpouseLastName = "";

        // Act
        var validationResults = ValidateModel(model);
        var hasSpouseError = validationResults.Any(v => 
            v.MemberNames.Contains(nameof(ApplicationFormModel.SpouseFirstName)) ||
            v.MemberNames.Contains(nameof(ApplicationFormModel.SpouseLastName)));

        // Assert
        hasSpouseError.Should().BeFalse("Spouse fields should be optional when not married");
    }

    [Fact]
    public void CurrentAddress_RequiredFields_ShouldFailValidation_WhenEmpty()
    {
        // Arrange
        var model = CreateValidModel();
        model.CurrentStreetAddress = "";
        model.CurrentCity = "";
        model.CurrentState = "";
        model.CurrentZipCode = "";

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.CurrentStreetAddress)));
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.CurrentCity)));
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.CurrentState)));
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.CurrentZipCode)));
    }

    [Fact]
    public void MilitaryService_RequiredFields_ShouldFailValidation_WhenEmpty()
    {
        // Arrange
        var model = CreateValidModel();
        model.MilitaryBranch = "";
        model.MilitaryStatus = "";

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.MilitaryBranch)));
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.MilitaryStatus)));
    }

    [Fact]
    public void PropertyInfo_RequiredFields_ShouldFailValidation_WhenEmpty()
    {
        // Arrange
        var model = CreateValidModel();
        model.PropertyAddress = "";
        model.City = "";
        model.State = "";
        model.ZipCode = "";

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.PropertyAddress)));
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.City)));
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.State)));
        validationResults.Should().Contain(v => v.MemberNames.Contains(nameof(ApplicationFormModel.ZipCode)));
    }

    [Theory]
    [InlineData("SingleFamily")]
    [InlineData("Condo")]
    [InlineData("Townhouse")]
    [InlineData("MultiUnit")]
    public void PropertyType_ShouldAccept_ValidTypes(string propertyType)
    {
        // Arrange
        var model = CreateValidModel();
        model.PropertyType = propertyType;

        // Act
        var validationResults = ValidateModel(model);
        var hasPropertyTypeError = validationResults.Any(v => v.MemberNames.Contains(nameof(ApplicationFormModel.PropertyType)));

        // Assert
        hasPropertyTypeError.Should().BeFalse($"Property type '{propertyType}' should be valid");
    }

    [Theory]
    [InlineData("PrimaryResidence")]
    [InlineData("SecondHome")]
    [InlineData("Investment")]
    public void Occupancy_ShouldAccept_ValidTypes(string occupancy)
    {
        // Arrange
        var model = CreateValidModel();
        model.Occupancy = occupancy;

        // Act
        var validationResults = ValidateModel(model);
        var hasOccupancyError = validationResults.Any(v => v.MemberNames.Contains(nameof(ApplicationFormModel.Occupancy)));

        // Assert
        hasOccupancyError.Should().BeFalse($"Occupancy '{occupancy}' should be valid");
    }

    #region Helper Methods

    private ApplicationFormModel CreateValidModel()
    {
        return new ApplicationFormModel
        {
            // Personal Info
            FirstName = "John",
            LastName = "Smith",
            DateOfBirth = new DateTime(1980, 1, 15),
            SSN = "123-45-6789",
            PhoneNumber = "(555) 123-4567",
            Email = "john.smith@example.com",
            CitizenshipStatus = "USCitizen",
            MaritalStatus = "Unmarried",

            // Current Address
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

            // Property Info
            PropertyAddress = "123 Main St",
            City = "San Diego",
            State = "CA",
            ZipCode = "92101",
            PropertyType = "SingleFamily",
            Occupancy = "PrimaryResidence",

            // Declarations
            IntendToOccupyProperty = true,
            HasBankruptcy = false,
            HasForeclosure = false
        };
    }

    private List<ValidationResult> ValidateModel(ApplicationFormModel model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    #endregion
}

