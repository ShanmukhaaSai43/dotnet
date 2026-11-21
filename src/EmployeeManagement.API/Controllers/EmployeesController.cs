using EmployeeManagement.API.DTOs;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;
    private static int _nextId = 200;

    public EmployeesController(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// GET /api/employees - Retrieves all employees with optional department or minimum pay filtering.
    /// Returns: 200 OK
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] int? departmentId = null, [FromQuery] decimal? minPay = null)
    {
        var query = _repository.GetAll().AsEnumerable();

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        if (minPay.HasValue)
        {
            query = query.Where(e => e.CalculateMonthlyPay() >= minPay.Value);
        }

        var responses = query.Select(MapToResponse);
        return Ok(responses);
    }

    /// <summary>
    /// GET /api/employees/{id} - Retrieves a single employee by unique ID.
    /// Returns: 200 OK or 404 Not Found
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var employee = _repository.GetById(id);
        if (employee == null)
        {
            return NotFound(new { message = $"Employee with ID '{id}' was not found." });
        }

        return Ok(MapToResponse(employee));
    }

    /// <summary>
    /// POST /api/employees - Creates a new employee record.
    /// Returns: 201 Created with Location header, or 400 Bad Request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] CreateEmployeeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check email uniqueness
        if (_repository.GetByEmail(request.Email) != null)
        {
            return BadRequest(new { message = $"An employee with email '{request.Email}' already exists." });
        }

        Address? address = !string.IsNullOrWhiteSpace(request.City)
            ? new Address(request.Street ?? "", request.City, request.State ?? "", request.ZipCode ?? "")
            : null;

        int newId = Interlocked.Increment(ref _nextId);
        Employee employee;

        if (request.Type == EmployeeType.FullTime)
        {
            employee = new FullTimeEmployee(
                id: newId,
                name: request.Name,
                email: request.Email,
                departmentId: request.DepartmentId,
                annualSalary: request.SalaryOrHourlyRate,
                monthlyBenefitsAllowance: request.MonthlyBenefits,
                address: address
            );
        }
        else
        {
            employee = new ContractEmployee(
                id: newId,
                name: request.Name,
                email: request.Email,
                departmentId: request.DepartmentId,
                hourlyRate: request.SalaryOrHourlyRate,
                hoursWorkedThisMonth: request.ContractHoursThisMonth,
                contractEndDate: request.ContractEndDate ?? DateTime.UtcNow.AddMonths(6),
                address: address
            );
        }

        _repository.Add(employee);

        var response = MapToResponse(employee);
        // Returns HTTP 201 Created with 'Location' header pointing to GET /api/employees/{id}
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, response);
    }

    /// <summary>
    /// PUT /api/employees/{id} - Full update of an existing employee.
    /// Returns: 204 No Content, 400 Bad Request, or 404 Not Found
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] UpdateEmployeeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = _repository.GetById(id);
        if (existing == null)
        {
            return NotFound(new { message = $"Employee with ID '{id}' was not found." });
        }

        // In a real database/EF Core, entity properties are updated. Here we update in repository
        Address? address = !string.IsNullOrWhiteSpace(request.City)
            ? new Address(request.Street ?? "", request.City, request.State ?? "", request.ZipCode ?? "")
            : existing.Address;

        existing.DepartmentId = request.DepartmentId;
        existing.Address = address;
        _repository.Update(existing);

        return NoContent(); // HTTP 204 No Content
    }

    /// <summary>
    /// PATCH /api/employees/{id}/salary - Partial update of an employee's compensation.
    /// Returns: 200 OK with updated employee, or 404 Not Found
    /// </summary>
    [HttpPatch("{id:int}/salary")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult PatchSalary(int id, [FromBody] PatchSalaryRequest request)
    {
        var existing = _repository.GetById(id);
        if (existing == null)
        {
            return NotFound(new { message = $"Employee with ID '{id}' was not found." });
        }

        if (existing is FullTimeEmployee ft)
        {
            ft.UpdateSalary(request.NewSalaryOrRate);
            _repository.Update(ft);
        }
        else if (existing is ContractEmployee ct)
        {
            // Update contract rate
            _repository.Delete(ct.Id);
            var updated = new ContractEmployee(ct.Id, ct.Name, ct.Email, ct.DepartmentId, request.NewSalaryOrRate, ct.HoursWorkedThisMonth, ct.ContractEndDate, ct.Address);
            _repository.Add(updated);
            existing = updated;
        }

        return Ok(MapToResponse(existing));
    }

    /// <summary>
    /// DELETE /api/employees/{id} - Deletes or deactivates an employee.
    /// Returns: 204 No Content or 404 Not Found
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var existing = _repository.GetById(id);
        if (existing == null)
        {
            return NotFound(new { message = $"Employee with ID '{id}' was not found." });
        }

        _repository.Delete(id);
        return NoContent(); // HTTP 204 No Content
    }

    private static EmployeeResponse MapToResponse(Employee emp)
    {
        return new EmployeeResponse(
            emp.Id,
            emp.Name,
            emp.Email,
            emp.DepartmentId,
            emp.Type.ToString(),
            emp.CalculateMonthlyPay(),
            emp.Address?.City,
            emp.IsActive,
            emp.CreatedAt
        );
    }
}
