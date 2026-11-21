using EmployeeManagement.API.DTOs;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DepartmentsController : ControllerBase
{
    private static readonly List<Department> _departments = new()
    {
        new Department(1, "Software Engineering", "ENG", monthlyBudget: 50000m),
        new Department(2, "Product & Design", "PRD", monthlyBudget: 30000m),
        new Department(3, "Sales & Marketing", "MKT", monthlyBudget: 25000m)
    };
    private static int _nextDeptId = 4;

    private readonly IEmployeeRepository _employeeRepository;

    public DepartmentsController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    /// <summary>
    /// GET /api/departments - Retrieves all registered departments.
    /// Returns: 200 OK
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DepartmentResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var response = _departments.Select(d => new DepartmentResponse(
            d.Id,
            d.Name,
            d.Code,
            d.MonthlyBudget,
            d.Status.ToString(),
            _employeeRepository.GetByDepartment(d.Id).Count,
            d.CreatedAt
        ));

        return Ok(response);
    }

    /// <summary>
    /// GET /api/departments/{id} - Retrieves a single department by ID.
    /// Returns: 200 OK or 404 Not Found
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DepartmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var dept = _departments.FirstOrDefault(d => d.Id == id);
        if (dept == null)
        {
            return NotFound(new { message = $"Department with ID '{id}' was not found." });
        }

        var response = new DepartmentResponse(
            dept.Id,
            dept.Name,
            dept.Code,
            dept.MonthlyBudget,
            dept.Status.ToString(),
            _employeeRepository.GetByDepartment(dept.Id).Count,
            dept.CreatedAt
        );

        return Ok(response);
    }

    /// <summary>
    /// POST /api/departments - Creates a new department.
    /// Returns: 201 Created or 400 Bad Request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DepartmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] CreateDepartmentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (_departments.Any(d => string.Equals(d.Code, request.Code, StringComparison.OrdinalIgnoreCase)))
        {
            return BadRequest(new { message = $"A department with code '{request.Code}' already exists." });
        }

        int newId = Interlocked.Increment(ref _nextDeptId);
        var department = new Department(newId, request.Name, request.Code, request.MonthlyBudget);
        _departments.Add(department);

        var response = new DepartmentResponse(
            department.Id,
            department.Name,
            department.Code,
            department.MonthlyBudget,
            department.Status.ToString(),
            0,
            department.CreatedAt
        );

        return CreatedAtAction(nameof(GetById), new { id = department.Id }, response);
    }

    /// <summary>
    /// GET /api/departments/{id}/payroll - Detailed payroll and budget compliance report.
    /// Returns: 200 OK or 404 Not Found
    /// </summary>
    [HttpGet("{id:int}/payroll")]
    [ProducesResponseType(typeof(DepartmentPayrollSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetDepartmentPayroll(int id)
    {
        var dept = _departments.FirstOrDefault(d => d.Id == id);
        if (dept == null)
        {
            return NotFound(new { message = $"Department with ID '{id}' was not found." });
        }

        var employees = _employeeRepository.GetByDepartment(id);
        decimal totalPayroll = employees.Where(e => e.IsActive).Sum(e => e.CalculateMonthlyPay());

        var employeeResponses = employees.Select(e => new EmployeeResponse(
            e.Id,
            e.Name,
            e.Email,
            e.DepartmentId,
            e.Type.ToString(),
            e.CalculateMonthlyPay(),
            e.Address?.City,
            e.IsActive,
            e.CreatedAt
        )).ToList();

        var summary = new DepartmentPayrollSummaryResponse(
            DepartmentId: dept.Id,
            DepartmentName: dept.Name,
            MonthlyBudget: dept.MonthlyBudget,
            TotalMonthlyPayroll: totalPayroll,
            IsOverBudget: totalPayroll > dept.MonthlyBudget,
            ActiveHeadcount: employees.Count(e => e.IsActive),
            Employees: employeeResponses
        );

        return Ok(summary);
    }
}
