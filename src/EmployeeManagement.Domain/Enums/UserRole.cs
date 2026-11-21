namespace EmployeeManagement.Domain.Enums;

/// <summary>
/// System user roles for Role-Based Access Control (RBAC).
/// </summary>
public enum UserRole
{
    Admin = 1,
    Manager = 2,
    Employee = 3
}

/// <summary>
/// Strongly-typed string constants for role names and role policy combinations.
/// Used in [Authorize(Roles = Roles.Admin)] attributes throughout the Presentation layer.
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Employee = "Employee";

    /// <summary>
    /// Combined role policy: Admin or Manager
    /// </summary>
    public const string AdminOrManager = "Admin,Manager";

    /// <summary>
    /// Combined role policy: All authenticated users
    /// </summary>
    public const string All = "Admin,Manager,Employee";
}
