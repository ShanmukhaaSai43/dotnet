namespace EmployeeManagement.Application.DTOs;

/// <summary>
/// User login credentials request.
/// </summary>
public record LoginRequest(string Username, string Password);

/// <summary>
/// Authentication response containing the signed JWT token and user metadata.
/// </summary>
public record LoginResponse(
    string Token,
    string Username,
    string Email,
    string Role,
    DateTime ExpiresAt
);

/// <summary>
/// User registration request.
/// </summary>
public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string Role,
    int? DepartmentId = null
);

/// <summary>
/// Current authenticated user identity and claims response.
/// </summary>
public record UserInfoResponse(
    string Username,
    string Email,
    string Role,
    int? EmployeeId,
    IEnumerable<string> Claims
);
