using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// GET /api/employees - Retrieves all employees with optional department or minimum pay filtering.
    /// Returns: 200 OK
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? departmentId = null, [FromQuery] decimal? minPay = null)
    {
        var responses = await _employeeService.GetAllEmployeesAsync(departmentId, minPay);
        return Ok(responses);
    }

    /// <summary>
    /// GET /api/employees/{id} - Retrieves a single employee by unique ID.
    /// Returns: 200 OK or 404 Not Found (via GlobalExceptionHandler)
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _employeeService.GetEmployeeByIdAsync(id);
        return Ok(response);
    }

    /// <summary>
    /// POST /api/employees - Creates a new employee record validated with FluentValidation.
    /// Returns: 201 Created with Location header, 400 Bad Request, or 409 Conflict
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        var created = await _employeeService.CreateEmployeeAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// PUT /api/employees/{id} - Full update of an existing employee.
    /// Returns: 204 NoContent, 400 Bad Request, or 404 Not Found
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeRequest request)
    {
        await _employeeService.UpdateEmployeeAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// PATCH /api/employees/{id}/salary - Partial update of compensation.
    /// Returns: 200 OK with updated employee, 400 Bad Request, or 404 Not Found
    /// </summary>
    [HttpPatch("{id:int}/salary")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchSalary(int id, [FromBody] PatchSalaryRequest request)
    {
        var updated = await _employeeService.UpdateSalaryAsync(id, request);
        return Ok(updated);
    }

    /// <summary>
    /// DELETE /api/employees/{id} - Deletes an employee record.
    /// Returns: 204 NoContent or 404 Not Found
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeService.DeleteEmployeeAsync(id);
        return NoContent();
    }
}
