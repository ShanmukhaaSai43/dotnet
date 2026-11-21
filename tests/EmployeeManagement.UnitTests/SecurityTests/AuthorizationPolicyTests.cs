using System.Reflection;
using EmployeeManagement.API.Controllers;
using EmployeeManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.UnitTests.SecurityTests;

/// <summary>
/// Verifies through reflection that all Presentation controllers and actions
/// enforce the exact authorization policies, roles, and anonymous access rules required by Gate 2.
/// </summary>
public class AuthorizationPolicyTests
{
    [Fact]
    public void Roles_Constants_MatchStandardRoleNames()
    {
        Assert.Equal("Admin", Roles.Admin);
        Assert.Equal("Manager", Roles.Manager);
        Assert.Equal("Employee", Roles.Employee);
        Assert.Equal("Admin,Manager", Roles.AdminOrManager);
    }

    [Fact]
    public void EmployeesController_HasAuthorizeAttributeAtClassLevel()
    {
        var authorizeAttr = typeof(EmployeesController).GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorizeAttr);
    }

    [Fact]
    public void EmployeesController_CreateAction_RequiresAdminOrManagerRole()
    {
        var method = typeof(EmployeesController).GetMethod(nameof(EmployeesController.Create));
        Assert.NotNull(method);

        var authorizeAttr = method.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorizeAttr);
        Assert.Equal(Roles.AdminOrManager, authorizeAttr.Roles);
    }

    [Fact]
    public void EmployeesController_UpdateAction_RequiresAdminOrManagerRole()
    {
        var method = typeof(EmployeesController).GetMethod(nameof(EmployeesController.Update));
        Assert.NotNull(method);

        var authorizeAttr = method.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorizeAttr);
        Assert.Equal(Roles.AdminOrManager, authorizeAttr.Roles);
    }

    [Fact]
    public void EmployeesController_PatchSalaryAction_RequiresAdminRole()
    {
        var method = typeof(EmployeesController).GetMethod(nameof(EmployeesController.PatchSalary));
        Assert.NotNull(method);

        var authorizeAttr = method.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorizeAttr);
        Assert.Equal(Roles.Admin, authorizeAttr.Roles);
    }

    [Fact]
    public void EmployeesController_DeleteAction_RequiresAdminRole()
    {
        var method = typeof(EmployeesController).GetMethod(nameof(EmployeesController.Delete));
        Assert.NotNull(method);

        var authorizeAttr = method.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorizeAttr);
        Assert.Equal(Roles.Admin, authorizeAttr.Roles);
    }

    [Fact]
    public void DepartmentsController_HasAuthorizeAttributeAtClassLevel()
    {
        var authorizeAttr = typeof(DepartmentsController).GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorizeAttr);
    }

    [Fact]
    public void DepartmentsController_CreateAction_RequiresAdminRole()
    {
        var method = typeof(DepartmentsController).GetMethod(nameof(DepartmentsController.Create));
        Assert.NotNull(method);

        var authorizeAttr = method.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorizeAttr);
        Assert.Equal(Roles.Admin, authorizeAttr.Roles);
    }

    [Fact]
    public void DepartmentsController_GetPayrollSummaryAction_RequiresAdminOrManagerRole()
    {
        var method = typeof(DepartmentsController).GetMethod(nameof(DepartmentsController.GetPayrollSummary));
        Assert.NotNull(method);

        var authorizeAttr = method.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorizeAttr);
        Assert.Equal(Roles.AdminOrManager, authorizeAttr.Roles);
    }

    [Fact]
    public void AuthController_LoginAction_AllowsAnonymousAccess()
    {
        var method = typeof(AuthController).GetMethod(nameof(AuthController.Login));
        Assert.NotNull(method);

        var allowAnonymousAttr = method.GetCustomAttribute<AllowAnonymousAttribute>();
        Assert.NotNull(allowAnonymousAttr);
    }

    [Fact]
    public void AuthController_GetCurrentUserAction_RequiresAuthorize()
    {
        var method = typeof(AuthController).GetMethod(nameof(AuthController.GetCurrentUser));
        Assert.NotNull(method);

        var authorizeAttr = method.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorizeAttr);
    }
}
