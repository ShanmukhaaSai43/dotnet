using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Validators;
using EmployeeManagement.Domain.Enums;
using FluentValidation.TestHelper;

namespace EmployeeManagement.UnitTests.ApplicationTests;

public class CreateEmployeeRequestValidatorTests
{
    private readonly CreateEmployeeRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidFullTimeEmployee_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var request = new CreateEmployeeRequest(
            Name: "John Doe",
            Email: "john.doe@company.com",
            DepartmentId: 1,
            Type: EmployeeType.FullTime,
            SalaryOrHourlyRate: 75000m,
            MonthlyBenefits: 500m,
            ZipCode: "98101"
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ValidContractEmployee_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var request = new CreateEmployeeRequest(
            Name: "Jane Contractor",
            Email: "jane@contracting.com",
            DepartmentId: 2,
            Type: EmployeeType.Contract,
            SalaryOrHourlyRate: 85m,
            ContractHoursThisMonth: 160,
            ZipCode: "98101-1234"
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyName_ShouldHaveValidationError(string? name)
    {
        // Arrange
        var request = new CreateEmployeeRequest(
            Name: name!,
            Email: "test@company.com",
            DepartmentId: 1,
            Type: EmployeeType.FullTime,
            SalaryOrHourlyRate: 60000m
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("John@Doe")]
    [InlineData("John123")]
    [InlineData("Name$Special")]
    public void Validate_InvalidCharactersInName_ShouldHaveValidationError(string invalidName)
    {
        // Arrange
        var request = new CreateEmployeeRequest(
            Name: invalidName,
            Email: "test@company.com",
            DepartmentId: 1,
            Type: EmployeeType.FullTime,
            SalaryOrHourlyRate: 60000m
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@domain")]
    [InlineData("@nodomain.com")]
    public void Validate_InvalidEmail_ShouldHaveValidationError(string invalidEmail)
    {
        // Arrange
        var request = new CreateEmployeeRequest(
            Name: "Valid Name",
            Email: invalidEmail,
            DepartmentId: 1,
            Type: EmployeeType.FullTime,
            SalaryOrHourlyRate: 60000m
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_InvalidDepartmentId_ShouldHaveValidationError(int invalidDeptId)
    {
        // Arrange
        var request = new CreateEmployeeRequest(
            Name: "Valid Name",
            Email: "valid@email.com",
            DepartmentId: invalidDeptId,
            Type: EmployeeType.FullTime,
            SalaryOrHourlyRate: 60000m
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DepartmentId);
    }

    [Fact]
    public void Validate_FullTimeSalaryBelowMinimum_ShouldHaveValidationError()
    {
        // Arrange (Salary < $20,000 for FullTime)
        var request = new CreateEmployeeRequest(
            Name: "Low Earner",
            Email: "low@company.com",
            DepartmentId: 1,
            Type: EmployeeType.FullTime,
            SalaryOrHourlyRate: 15000m
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SalaryOrHourlyRate);
    }

    [Fact]
    public void Validate_ContractHourlyRateBelowMinimum_ShouldHaveValidationError()
    {
        // Arrange (Hourly rate < $15 for Contract)
        var request = new CreateEmployeeRequest(
            Name: "Contractor",
            Email: "contractor@company.com",
            DepartmentId: 1,
            Type: EmployeeType.Contract,
            SalaryOrHourlyRate: 10m
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SalaryOrHourlyRate);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("ABCDE")]
    [InlineData("123456")]
    public void Validate_InvalidZipCode_ShouldHaveValidationError(string invalidZip)
    {
        // Arrange
        var request = new CreateEmployeeRequest(
            Name: "Valid Name",
            Email: "valid@email.com",
            DepartmentId: 1,
            Type: EmployeeType.FullTime,
            SalaryOrHourlyRate: 60000m,
            ZipCode: invalidZip
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ZipCode);
    }
}
