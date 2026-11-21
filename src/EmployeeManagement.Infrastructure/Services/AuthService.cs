using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Enums;
using FluentValidation;
using ValidationException = EmployeeManagement.Application.Common.Exceptions.ValidationException;

namespace EmployeeManagement.Infrastructure.Services;

/// <summary>
/// Infrastructure authentication service verifying user credentials and issuing JWT tokens.
/// Pre-seeded with administrative and role-based test accounts for training and evaluation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IValidator<LoginRequest> _validator;

    // Pre-seeded demo user registry for Capstone assessment
    private static readonly Dictionary<string, (string Password, string Email, string Role, int? EmployeeId)> DemoUsers =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["admin"] = ("Admin@123", "admin@company.com", Roles.Admin, 1),
            ["manager"] = ("Manager@123", "manager@company.com", Roles.Manager, 2),
            ["employee"] = ("Employee@123", "employee@company.com", Roles.Employee, 3)
        };

    public AuthService(IJwtTokenService jwtTokenService, IValidator<LoginRequest> validator)
    {
        _jwtTokenService = jwtTokenService;
        _validator = validator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        if (!DemoUsers.TryGetValue(request.Username, out var user) || user.Password != request.Password)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var token = _jwtTokenService.GenerateToken(request.Username, user.Email, user.Role, user.EmployeeId);
        var expiresAt = DateTime.UtcNow.AddMinutes(60);

        return new LoginResponse(token, request.Username, user.Email, user.Role, expiresAt);
    }
}
