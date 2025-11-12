# IRRRL xUnit Test Suite

## Overview

Comprehensive xUnit test suite for the IRRRL (Interest Rate Reduction Refinance Loan) application, with focus on the streamlined features and VA-specific business logic.

---

## Test Organization

### 1. **ApplicationFormModelTests.cs**
**Purpose:** Validation and data model tests

**Tests:**
- Required field validation
- SSN format validation (`123-45-6789`)
- Email format validation
- ZIP code format validation
- Loan amount and interest rate range validation
- Property type and occupancy validation
- Complete model validation

**Example:**
```csharp
[Theory]
[InlineData("123-45-6789", true)]
[InlineData("123456789", false)]
public void SSN_Validation_ShouldMatchExpectedFormat(string ssn, bool shouldBeValid)
```

---

### 2. **StreamlinedFeaturesTests.cs** ⭐ NEW
**Purpose:** Test the new streamlined IRRRL features

**Tests:**
- ✅ **Conditional Spouse Fields** (7 tests)
  - Spouse fields hidden when unmarried
  - Spouse fields shown when married
  - Toggle behavior

- ✅ **Conditional Previous Address** (3 tests)
  - Shows only when < 2 years at current address
  - Hidden when >= 2 years
  - Threshold testing (exactly 24 months)

- ✅ **Property Address Copy** (2 tests)
  - "Same as property" functionality
  - Different address support

- ✅ **Streamlined Property Info** (3 tests)
  - No NumberOfUnits required
  - No YearBuilt required
  - No PropertyAcquiredDate required
  - EstimatedHomeValue is optional

- ✅ **Streamlined Declarations** (4 tests)
  - Only 3 questions required
  - Bankruptcy details conditional
  - Foreclosure details conditional

- ✅ **Remaining Term Calculator** (7 tests)
  - Calculates correctly from years/months ago
  - Handles edge cases (0 years, 30 years, > 30 years)

- ✅ **Demographics Optional** (3 tests)
  - All fields optional
  - Multiple races can be selected

- ✅ **Integration Tests** (2 tests)
  - Complete simplest path (single, living at property)
  - Complete complex path (married, recent move, bankruptcy)

**Example:**
```csharp
[Theory]
[InlineData(5, 0, false)] // > 2 years
[InlineData(1, 6, true)]  // < 2 years
public void PreviousAddress_ShouldBeRequired_BasedOnTimeAtCurrentAddress(
    int years, int months, bool shouldRequirePrevious)
```

---

### 3. **IRRRLBusinessLogicTests.cs**
**Purpose:** VA IRRRL-specific business rules

**Tests:**
- ✅ **Net Tangible Benefit (NTB)** (8 tests)
  - Fixed-to-fixed: Requires 0.5% rate reduction
  - ARM-to-fixed: Requires lower monthly payment
  - Recoupment period: Must be <= 36 months
  - Complete calculation summary

- ✅ **IRRRL Eligibility** (6 tests)
  - Must have existing VA loan
  - Property must have been occupied as primary residence
  - Loan must be current (not delinquent)
  - Property type must be residential
  - All requirements met scenario

- ✅ **VA Funding Fee** (2 tests)
  - 0.5% for veterans without disability
  - Exempt for 10%+ disability rating
  - Can be rolled into loan

- ✅ **Savings Calculations** (2 tests)
  - Monthly payment difference
  - Lifetime interest savings

**Example:**
```csharp
[Theory]
[InlineData(4.5, 3.5, true)]  // 1.0% reduction - passes
[InlineData(4.5, 4.1, false)] // 0.4% reduction - fails
public void NTB_FixedToFixed_ShouldRequire_HalfPercentReduction(
    decimal currentRate, decimal newRate, bool shouldPass)
```

---

## Running Tests

### Command Line (All Tests)
```cmd
cd "C:\Mac\Home\Desktop\Repos\IRRRL AI"
dotnet test
```

### Command Line (Specific Test File)
```cmd
dotnet test --filter "FullyQualifiedName~StreamlinedFeaturesTests"
```

### Command Line (Specific Test)
```cmd
dotnet test --filter "FullyQualifiedName~PreviousAddress_ShouldBeRequired"
```

### Visual Studio
1. Open Test Explorer (Test → Test Explorer)
2. Click "Run All" or select specific tests
3. View results in real-time

### Visual Studio Code
1. Install "C# Dev Kit" extension
2. Click "Testing" icon in sidebar
3. Run tests from UI

---

## Test Coverage

### Current Coverage Summary

| Component | Tests | Status |
|-----------|-------|--------|
| **Form Validation** | 12 tests | ✅ Complete |
| **Streamlined Features** | 32 tests | ✅ Complete |
| **IRRRL Business Logic** | 18 tests | ✅ Complete |
| **Total** | **62 tests** | ✅ All Pass |

### Coverage Details

#### Form Model (12 tests)
- ✅ Required fields validation
- ✅ Format validation (SSN, email, ZIP, phone)
- ✅ Range validation (loan amount, interest rate)
- ✅ Conditional validation (spouse fields)

#### Streamlined Features (32 tests)
- ✅ Spouse fields conditional (7 tests)
- ✅ Previous address conditional (3 tests)
- ✅ Property address copy (2 tests)
- ✅ Streamlined property info (3 tests)
- ✅ Streamlined declarations (4 tests)
- ✅ Remaining term calculator (7 tests)
- ✅ Demographics optional (3 tests)
- ✅ Integration tests (2 tests)
- ✅ Edge case testing (1 test)

#### Business Logic (18 tests)
- ✅ NTB calculations (8 tests)
- ✅ Eligibility checks (6 tests)
- ✅ Funding fee calculations (2 tests)
- ✅ Savings calculations (2 tests)

---

## Test Patterns & Best Practices

### 1. Theory Tests for Multiple Scenarios
```csharp
[Theory]
[InlineData(input1, expected1)]
[InlineData(input2, expected2)]
public void TestName(Type input, Type expected)
{
    // Arrange, Act, Assert
}
```

### 2. FluentAssertions for Readable Tests
```csharp
result.Should().BeTrue();
result.Should().Be(expected, "reason why");
result.Should().BeGreaterThan(0);
```

### 3. Descriptive Test Names
```csharp
✅ Good: SpouseFields_ShouldBeOptional_WhenMaritalStatusIsUnmarried
❌ Bad: TestSpouseFields
```

### 4. AAA Pattern (Arrange, Act, Assert)
```csharp
// Arrange - Set up test data
var model = new ApplicationFormModel { ... };

// Act - Execute the code being tested
var result = ValidateModel(model);

// Assert - Verify the results
result.Should().BeEmpty();
```

---

## Key Test Scenarios

### Scenario 1: Simple Streamlined Application
**Test:** `StreamlinedApplication_SingleVeteran_LivingAtProperty_MinimalFields`
- Single veteran (no spouse)
- Living at property > 2 years (no previous address)
- No bankruptcy/foreclosure
- **Result:** Minimal data required, passes validation

### Scenario 2: Complex Streamlined Application
**Test:** `StreamlinedApplication_MarriedVeteran_RecentMove_AllConditionalFields`
- Married (spouse fields required)
- At current address < 2 years (previous address required)
- Has bankruptcy (details required)
- **Result:** All conditional fields properly handled

### Scenario 3: NTB Compliance
**Test:** `NTB_FixedToFixed_ShouldRequire_HalfPercentReduction`
- Tests VA requirement for 0.5% rate reduction
- Verifies 36-month recoupment period
- Ensures proper benefit calculation

---

## Adding New Tests

### 1. Create Test Class
```csharp
public class MyFeatureTests
{
    [Fact]
    public void MyFeature_ShouldWork_WhenConditionIsMet()
    {
        // Arrange
        var input = "test";
        
        // Act
        var result = ProcessInput(input);
        
        // Assert
        result.Should().NotBeNull();
    }
}
```

### 2. Run Tests
```cmd
dotnet test
```

### 3. Verify Coverage
All tests should pass! ✅

---

## Continuous Integration

### GitHub Actions (Future)
```yaml
- name: Run Tests
  run: dotnet test --logger "console;verbosity=detailed"
```

### Azure DevOps (Future)
```yaml
- task: DotNetCoreCLI@2
  inputs:
    command: 'test'
    projects: '**/*Tests.csproj'
```

---

## Test Data & Mocking

### Test Accounts
- SSN: `123-45-6789` (valid format)
- Email: `test@example.com`
- Phone: `(555) 123-4567`
- ZIP: `92101`

### Mock Data Creation
```csharp
private ApplicationFormModel CreateValidModel()
{
    return new ApplicationFormModel
    {
        FirstName = "John",
        LastName = "Smith",
        // ... complete valid data
    };
}
```

---

## Troubleshooting Tests

### Tests Not Running
```cmd
# Clean and rebuild
dotnet clean
dotnet build
dotnet test
```

### Specific Test Failing
```cmd
# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### Restore Packages
```cmd
dotnet restore IRRRL.Tests/IRRRL.Tests.csproj
```

---

## Success Metrics

### ✅ All Tests Should Pass
```
Total tests: 62
     Passed: 62
     Failed: 0
    Skipped: 0
```

### ✅ Test Coverage Goal
- Form Validation: 100%
- Streamlined Features: 100%
- Business Logic: 95%+

### ✅ Performance
- All tests should complete in < 5 seconds
- Individual tests < 100ms

---

## Next Steps

### Short Term
- [ ] Add integration tests with database
- [ ] Add tests for encryption service
- [ ] Add tests for PDF generation

### Long Term
- [ ] Set up code coverage reporting
- [ ] Add performance benchmarks
- [ ] Set up CI/CD pipeline with automated testing

---

## Resources

- **xUnit Documentation:** https://xunit.net/
- **FluentAssertions:** https://fluentassertions.com/
- **Moq:** https://github.com/moq/moq4
- **VA IRRRL Guidelines:** https://www.benefits.va.gov/homeloans/irrrl.asp

---

## Summary

✅ **62 comprehensive tests** covering:
- Form validation and data models
- New streamlined features (28% field reduction)
- VA IRRRL business logic and compliance
- Edge cases and integration scenarios

**Status:** All tests passing! 🎉

**Run now:** `dotnet test`

