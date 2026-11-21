using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using FluentValidation;
using ValidationException = EmployeeManagement.Application.Common.Exceptions.ValidationException;

namespace EmployeeManagement.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IEmployeeRepository _repository;
    private readonly IPayrollCalculator _payrollCalculator;
    private readonly IValidator<CreateDepartmentRequest> _createValidator;

    // In-memory department store (mirrors existing controller store)
    private static readonly List<Department> _departments = new()
    {
        new Department(1, "Engineering", "ENG-01", 150000m),
        new Department(2, "Human Resources", "HR-01", 50000m),
        new Department(3, "Finance", "FIN-01", 80000m)
    };
    private static int _nextDeptId = 4;
    private static readonly object _lock = new();

    public DepartmentService(
        IEmployeeRepository repository,
        IPayrollCalculator payrollCalculator,
        IValidator<CreateDepartmentRequest> createValidator)
    {
        _repository = repository;
        _payrollCalculator = payrollCalculator;
        _createValidator = createValidator;
    }

    public Task<IEnumerable<DepartmentResponse>> GetAllDepartmentsAsync()
    {
        var allEmployees = _repository.GetAll();
        var responses = _departments.Select(d => new DepartmentResponse(
            Id: d.Id,
            Name: d.Name,
            Code: d.Code,
            MonthlyBudget: d.MonthlyBudget,
            Status: d.Status.ToString(),
            EmployeeCount: allEmployees.Count(e => e.DepartmentId == d.Id),
            CreatedAt: d.CreatedAt
        ));

        return Task.FromResult<IEnumerable<DepartmentResponse>>(responses.ToList());
    }

    public Task<DepartmentResponse> GetDepartmentByIdAsync(int id)
    {
        var department = _departments.FirstOrDefault(d => d.Id == id)
            ?? throw new NotFoundException("Department", id);

        var count = _repository.GetAll().Count(e => e.DepartmentId == id);
        return Task.FromResult(new DepartmentResponse(
            Id: department.Id,
            Name: department.Name,
            Code: department.Code,
            MonthlyBudget: department.MonthlyBudget,
            Status: department.Status.ToString(),
            EmployeeCount: count,
            CreatedAt: department.CreatedAt
        ));
    }

    public async Task<DepartmentResponse> CreateDepartmentAsync(CreateDepartmentRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var codeExists = _departments.Any(d => d.Code.Equals(request.Code.Trim(), StringComparison.OrdinalIgnoreCase));
        if (codeExists)
        {
            throw new ConflictException($"Department with code '{request.Code}' already exists.");
        }

        int id;
        lock (_lock)
        {
            id = _nextDeptId++;
        }

        var department = new Department(id, request.Name.Trim(), request.Code.Trim().ToUpperInvariant(), request.MonthlyBudget);
        _departments.Add(department);

        return new DepartmentResponse(
            Id: department.Id,
            Name: department.Name,
            Code: department.Code,
            MonthlyBudget: department.MonthlyBudget,
            Status: department.Status.ToString(),
            EmployeeCount: 0,
            CreatedAt: department.CreatedAt
        );
    }

    public Task<DepartmentPayrollSummaryResponse> GetDepartmentPayrollSummaryAsync(int departmentId)
    {
        var department = _departments.FirstOrDefault(d => d.Id == departmentId)
            ?? throw new NotFoundException("Department", departmentId);

        var deptEmployees = _repository.GetAll()
            .Where(e => e.DepartmentId == departmentId && e.IsActive)
            .ToList();

        decimal totalPayroll = deptEmployees.Sum(e => e.CalculateMonthlyPay());
        bool isOverBudget = totalPayroll > department.MonthlyBudget;

        var employeeResponses = deptEmployees.Select(emp => new EmployeeResponse(
            Id: emp.Id,
            Name: emp.Name,
            Email: emp.Email,
            DepartmentId: emp.DepartmentId,
            Type: emp is FullTimeEmployee ? "FullTime" : "Contract",
            MonthlyPay: Math.Round(emp.CalculateMonthlyPay(), 2),
            City: emp.Address?.City,
            IsActive: emp.IsActive,
            CreatedAt: emp.CreatedAt
        )).ToList();

        return Task.FromResult(new DepartmentPayrollSummaryResponse(
            DepartmentId: department.Id,
            DepartmentName: department.Name,
            MonthlyBudget: department.MonthlyBudget,
            TotalMonthlyPayroll: Math.Round(totalPayroll, 2),
            IsOverBudget: isOverBudget,
            ActiveHeadcount: deptEmployees.Count,
            Employees: employeeResponses
        ));
    }
}
