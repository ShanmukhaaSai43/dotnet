using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize] // Requires authenticated JWT token for all endpoints by default
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// GET /api/employees - Retrieves all employees with optional department or minimum pay filtering.
    /// Authorization: Any authenticated role (Admin, Manager, Employee).
    /// Returns: 200 OK
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll([FromQuery] int? departmentId = null, [FromQuery] decimal? minPay = null)
    {
        var responses = await _employeeService.GetAllEmployeesAsync(departmentId, minPay);
        return Ok(responses);
    }

    /// <summary>
    /// GET /api/employees/paged - Server-side paginated, filtered, and sorted employee list.
    /// Authorization: Any authenticated role (Admin, Manager, Employee).
    /// Returns: 200 OK with PagedResult<EmployeeResponse>
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<EmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPaged([FromQuery] PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await _employeeService.GetPagedEmployeesAsync(pagination, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/employees/{id} - Retrieves a single employee by unique ID.
    /// Authorization: Any authenticated role (Admin, Manager, Employee).
    /// Returns: 200 OK or 404 Not Found (via GlobalExceptionHandler)
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _employeeService.GetEmployeeByIdAsync(id);
        return Ok(response);
    }

    /// <summary>
    /// POST /api/employees - Creates a new employee record validated with FluentValidation.
    /// Authorization: Admin or Manager only.
    /// Returns: 201 Created with Location header, 400 Bad Request, 401 Unauthorized, 403 Forbidden, or 409 Conflict
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        var created = await _employeeService.CreateEmployeeAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// PUT /api/employees/{id} - Full update of an existing employee.
    /// Authorization: Admin or Manager only.
    /// Returns: 204 NoContent, 400 Bad Request, 401 Unauthorized, 403 Forbidden, or 404 Not Found
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeRequest request)
    {
        await _employeeService.UpdateEmployeeAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// PATCH /api/employees/{id}/salary - Partial update of compensation.
    /// Authorization: Admin only.
    /// Returns: 200 OK with updated employee, 400 Bad Request, 401 Unauthorized, 403 Forbidden, or 404 Not Found
    /// </summary>
    [HttpPatch("{id:int}/salary")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchSalary(int id, [FromBody] PatchSalaryRequest request)
    {
        var updated = await _employeeService.UpdateSalaryAsync(id, request);
        return Ok(updated);
    }

    /// <summary>
    /// DELETE /api/employees/{id} - Deletes an employee record.
    /// Authorization: Admin only.
    /// Returns: 204 NoContent, 401 Unauthorized, 403 Forbidden, or 404 Not Found
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeService.DeleteEmployeeAsync(id);
        return NoContent();
    }
}
