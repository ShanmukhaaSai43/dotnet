using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.Interfaces;

/// <summary>
/// Service abstraction for user authentication and token issuance.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Validates credentials and returns a signed JWT token with user details.
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
