using System.Security.Claims;

namespace EmployeeManagement.Application.Interfaces;

/// <summary>
/// Service abstraction for issuing and validating JSON Web Tokens (JWT).
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a signed HMAC-SHA256 JWT string containing standard claims and role.
    /// </summary>
    string GenerateToken(string username, string email, string role, int? employeeId = null);

    /// <summary>
    /// Validates a JWT string and returns its ClaimsPrincipal if valid; otherwise null.
    /// </summary>
    ClaimsPrincipal? ValidateToken(string token);
}
