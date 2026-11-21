using System.Security.Claims;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// POST /api/auth/login - Authenticates credentials and returns a signed HMAC-SHA256 JWT Bearer token.
    /// Pre-configured test accounts:
    ///   - admin    / Admin@123    (Role: Admin)
    ///   - manager  / Manager@123  (Role: Manager)
    ///   - employee / Employee@123 (Role: Employee)
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// GET /api/auth/me - Returns current authenticated user claims and permissions.
    /// Requires any valid Bearer token.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(ClaimTypes.Name)
            ?? User.Identity?.Name
            ?? "Unknown";

        var email = User.FindFirstValue(ClaimTypes.Email) ?? "Unknown";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "None";
        int? employeeId = int.TryParse(User.FindFirstValue("employee_id"), out var id) ? id : null;

        var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();

        return Ok(new UserInfoResponse(username, email, role, employeeId, claims));
    }
}
