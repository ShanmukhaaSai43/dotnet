using EmployeeManagement.API.Configuration;
using EmployeeManagement.API.Services.Lifetimes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DiagnosticsController : ControllerBase
{
    private readonly IOperationTransient _transient1;
    private readonly IOperationTransient _transient2;
    private readonly IOperationScoped _scoped1;
    private readonly IOperationScoped _scoped2;
    private readonly IOperationSingleton _singleton1;
    private readonly IOperationSingleton _singleton2;
    private readonly PayrollSettings _payrollSettings;

    public DiagnosticsController(
        IOperationTransient transient1,
        IOperationTransient transient2,
        IOperationScoped scoped1,
        IOperationScoped scoped2,
        IOperationSingleton singleton1,
        IOperationSingleton singleton2,
        IOptions<PayrollSettings> payrollSettings)
    {
        _transient1 = transient1;
        _transient2 = transient2;
        _scoped1 = scoped1;
        _scoped2 = scoped2;
        _singleton1 = singleton1;
        _singleton2 = singleton2;
        _payrollSettings = payrollSettings.Value;
    }

    /// <summary>
    /// GET /api/diagnostics/lifetimes - Visually demonstrates DI lifetimes.
    /// Injects two instances of Transient, Scoped, and Singleton in the same request.
    /// </summary>
    [HttpGet("lifetimes")]
    public IActionResult GetLifetimes()
    {
        return Ok(new
        {
            TransientInstances = new
            {
                Instance1 = _transient1.OperationId,
                Instance2 = _transient2.OperationId,
                AreEqual = _transient1.OperationId == _transient2.OperationId,
                Explanation = "Transient instances are ALWAYS DIFFERENT (new instance per resolution)"
            },
            ScopedInstances = new
            {
                Instance1 = _scoped1.OperationId,
                Instance2 = _scoped2.OperationId,
                AreEqual = _scoped1.OperationId == _scoped2.OperationId,
                Explanation = "Scoped instances are IDENTICAL within the same HTTP request"
            },
            SingletonInstances = new
            {
                Instance1 = _singleton1.OperationId,
                Instance2 = _singleton2.OperationId,
                AreEqual = _singleton1.OperationId == _singleton2.OperationId,
                Explanation = "Singleton instances are IDENTICAL across all HTTP requests and users"
            }
        });
    }

    /// <summary>
    /// GET /api/diagnostics/configuration - Demonstrates IOptions<PayrollSettings> strongly-typed configuration.
    /// </summary>
    [HttpGet("configuration")]
    public IActionResult GetConfiguration()
    {
        return Ok(new
        {
            Source = "appsettings.json via IOptions<PayrollSettings>",
            _payrollSettings.CompanyName,
            _payrollSettings.DefaultBonusRate,
            _payrollSettings.MinimumAnnualSalary,
            _payrollSettings.OvertimeMultiplier,
            _payrollSettings.AutoApproveBenefits
        });
    }
}
