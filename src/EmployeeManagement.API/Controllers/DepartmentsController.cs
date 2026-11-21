using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    /// <summary>
    /// GET /api/departments - Retrieves all registered departments.
    /// Returns: 200 OK
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DepartmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await _departmentService.GetAllDepartmentsAsync();
        return Ok(response);
    }

    /// <summary>
    /// GET /api/departments/{id} - Retrieves a single department by ID.
    /// Returns: 200 OK or 404 Not Found
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DepartmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _departmentService.GetDepartmentByIdAsync(id);
        return Ok(response);
    }

    /// <summary>
    /// POST /api/departments - Creates a new department validated with FluentValidation.
    /// Returns: 201 Created or 400 Bad Request / 409 Conflict
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DepartmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        var created = await _departmentService.CreateDepartmentAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// GET /api/departments/{id}/payroll - Calculates total monthly payroll and budget variance.
    /// Returns: 200 OK or 404 Not Found
    /// </summary>
    [HttpGet("{id:int}/payroll")]
    [ProducesResponseType(typeof(DepartmentPayrollSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayrollSummary(int id)
    {
        var summary = await _departmentService.GetDepartmentPayrollSummaryAsync(id);
        return Ok(summary);
    }
}
