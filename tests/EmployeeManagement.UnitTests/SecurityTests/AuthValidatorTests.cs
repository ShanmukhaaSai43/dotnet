using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Validators;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.UnitTests.SecurityTests;

public class AuthValidatorTests
{
    private readonly LoginRequestValidator _loginValidator = new();
    private readonly RegisterRequestValidator _registerValidator = new();

    [Fact]
    public void LoginValidator_WithValidCredentials_Passes()
    {
        var request = new LoginRequest("valid_user", "SecurePassword123!");
        var result = _loginValidator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void LoginValidator_WithEmptyUsername_Fails(string? username)
    {
        var request = new LoginRequest(username!, "Password123!");
        var result = _loginValidator.Validate(request);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginRequest.Username));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void LoginValidator_WithEmptyPassword_Fails(string? password)
    {
        var request = new LoginRequest("valid_user", password!);
        var result = _loginValidator.Validate(request);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginRequest.Password));
    }

    [Fact]
    public void RegisterValidator_WithValidInput_Passes()
    {
        var request = new RegisterRequest("newuser", "user@company.com", "SecurePass123", Roles.Employee);
        var result = _registerValidator.Validate(request);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("plainaddress")]
    [InlineData("@nodomain.com")]
    public void RegisterValidator_WithInvalidEmail_Fails(string invalidEmail)
    {
        var request = new RegisterRequest("newuser", invalidEmail, "SecurePass123", Roles.Employee);
        var result = _registerValidator.Validate(request);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Email));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("short")]
    public void RegisterValidator_WithShortPassword_Fails(string shortPassword)
    {
        var request = new RegisterRequest("newuser", "user@company.com", shortPassword, Roles.Employee);
        var result = _registerValidator.Validate(request);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Password));
    }

    [Theory]
    [InlineData("SuperUser")]
    [InlineData("Guest")]
    [InlineData("Hacker")]
    public void RegisterValidator_WithInvalidRole_Fails(string invalidRole)
    {
        var request = new RegisterRequest("newuser", "user@company.com", "SecurePass123", invalidRole);
        var result = _registerValidator.Validate(request);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Role));
    }
}
