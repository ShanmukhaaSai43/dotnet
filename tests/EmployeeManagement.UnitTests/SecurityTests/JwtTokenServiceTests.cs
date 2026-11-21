using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EmployeeManagement.Application.Configuration;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.UnitTests.SecurityTests;

public class JwtTokenServiceTests
{
    private readonly JwtSettings _jwtSettings;
    private readonly JwtTokenService _tokenService;

    public JwtTokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "TestSuperSecretKeyForUnitTestsMustBeLongEnough256Bits!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 60
        };

        var options = Options.Create(_jwtSettings);
        _tokenService = new JwtTokenService(options);
    }

    [Fact]
    public void GenerateToken_WithValidInput_ReturnsNonEmptyJwtString()
    {
        // Act
        var token = _tokenService.GenerateToken("testuser", "test@company.com", Roles.Admin);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(token));
    }

    [Fact]
    public void GenerateToken_IncludesAllStandardClaims()
    {
        // Act
        var token = _tokenService.GenerateToken("john_doe", "john@company.com", Roles.Manager);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Equal(_jwtSettings.Issuer, jwt.Issuer);
        Assert.Contains(_jwtSettings.Audience, jwt.Audiences);

        var subClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        Assert.NotNull(subClaim);
        Assert.Equal("john_doe", subClaim.Value);

        var emailClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email");
        Assert.NotNull(emailClaim);
        Assert.Equal("john@company.com", emailClaim.Value);

        var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");
        Assert.NotNull(roleClaim);
        Assert.Equal(Roles.Manager, roleClaim.Value);
    }

    [Fact]
    public void GenerateToken_WithEmployeeId_IncludesCustomEmployeeIdClaim()
    {
        // Act
        var token = _tokenService.GenerateToken("emp_user", "emp@company.com", Roles.Employee, employeeId: 42);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var empIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "employee_id");
        Assert.NotNull(empIdClaim);
        Assert.Equal("42", empIdClaim.Value);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsPopulatedPrincipal()
    {
        // Arrange
        var token = _tokenService.GenerateToken("alice", "alice@company.com", Roles.Admin);

        // Act
        var principal = _tokenService.ValidateToken(token);

        // Assert
        Assert.NotNull(principal);
        Assert.True(principal.Identity?.IsAuthenticated);
        Assert.Equal("alice", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal(Roles.Admin, principal.FindFirst(ClaimTypes.Role)?.Value);
    }

    [Fact]
    public void ValidateToken_WithTamperedToken_ReturnsNull()
    {
        // Arrange
        var token = _tokenService.GenerateToken("alice", "alice@company.com", Roles.Admin);
        var tamperedToken = token[..^5] + "XXXXX"; // corrupt signature

        // Act
        var principal = _tokenService.ValidateToken(tamperedToken);

        // Assert
        Assert.Null(principal);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-token")]
    public void ValidateToken_WithMalformedString_ReturnsNull(string invalidToken)
    {
        // Act
        var principal = _tokenService.ValidateToken(invalidToken);

        // Assert
        Assert.Null(principal);
    }

    [Fact]
    public void ValidateToken_WithDifferentKey_ReturnsNull()
    {
        // Arrange - generate with first key
        var token = _tokenService.GenerateToken("alice", "alice@company.com", Roles.Admin);

        // Service configured with different secret key
        var foreignSettings = new JwtSettings
        {
            SecretKey = "AnotherCompletelyDifferentSecretKeyForSigning256Bits!",
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            ExpiryMinutes = 60
        };
        var foreignService = new JwtTokenService(Options.Create(foreignSettings));

        // Act
        var principal = foreignService.ValidateToken(token);

        // Assert
        Assert.Null(principal);
    }

    [Fact]
    public void ValidateToken_WithExpiredToken_ReturnsNull()
    {
        // Arrange - token service with 0 or negative expiry
        var expiredSettings = new JwtSettings
        {
            SecretKey = _jwtSettings.SecretKey,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            ExpiryMinutes = -5 // Already expired 5 minutes ago
        };
        var expiredService = new JwtTokenService(Options.Create(expiredSettings));
        var token = expiredService.GenerateToken("alice", "alice@company.com", Roles.Admin);

        // Act
        var principal = _tokenService.ValidateToken(token);

        // Assert
        Assert.Null(principal);
    }
}
