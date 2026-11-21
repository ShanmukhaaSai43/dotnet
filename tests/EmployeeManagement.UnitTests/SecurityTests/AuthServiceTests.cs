using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Configuration;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Validators;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.UnitTests.SecurityTests;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly JwtTokenService _jwtTokenService;

    public AuthServiceTests()
    {
        var jwtSettings = new JwtSettings
        {
            SecretKey = "TestSuperSecretKeyForUnitTestsMustBeLongEnough256Bits!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 60
        };

        _jwtTokenService = new JwtTokenService(Options.Create(jwtSettings));
        var validator = new LoginRequestValidator();
        _authService = new AuthService(_jwtTokenService, validator);
    }

    [Fact]
    public async Task LoginAsync_WithValidAdminCredentials_ReturnsTokenAndAdminRole()
    {
        // Arrange
        var request = new LoginRequest("admin", "Admin@123");

        // Act
        var response = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
        Assert.Equal("admin", response.Username);
        Assert.Equal(Roles.Admin, response.Role);
        Assert.Equal("admin@company.com", response.Email);
        Assert.True(response.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_WithValidManagerCredentials_ReturnsTokenAndManagerRole()
    {
        // Arrange
        var request = new LoginRequest("manager", "Manager@123");

        // Act
        var response = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(Roles.Manager, response.Role);
        Assert.Equal("manager@company.com", response.Email);
    }

    [Fact]
    public async Task LoginAsync_WithValidEmployeeCredentials_ReturnsTokenAndEmployeeRole()
    {
        // Arrange
        var request = new LoginRequest("employee", "Employee@123");

        // Act
        var response = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(Roles.Employee, response.Role);
        Assert.Equal("employee@company.com", response.Email);
    }

    [Theory]
    [InlineData("admin", "WrongPassword!")]
    [InlineData("admin", "admin123")]
    [InlineData("manager", "admin")]
    public async Task LoginAsync_WithWrongPassword_ThrowsUnauthorizedAccessException(string username, string wrongPassword)
    {
        // Arrange
        var request = new LoginRequest(username, wrongPassword);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(request));
        Assert.Contains("Invalid username or password", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_WithUnknownUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest("non_existent_user", "AnyPassword123!");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(request));
        Assert.Contains("Invalid username or password", ex.Message);
    }

    [Theory]
    [InlineData("", "Admin@123")]
    [InlineData("admin", "")]
    [InlineData("", "")]
    public async Task LoginAsync_WithMissingFields_ThrowsValidationException(string username, string password)
    {
        // Arrange
        var request = new LoginRequest(username, password);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _authService.LoginAsync(request));
    }
}
