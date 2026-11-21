using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;
using FluentValidation;
using ValidationException = EmployeeManagement.Application.Common.Exceptions.ValidationException;

namespace EmployeeManagement.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IPayrollCalculator _payrollCalculator;
    private readonly IValidator<CreateEmployeeRequest> _createValidator;
    private readonly IValidator<UpdateEmployeeRequest> _updateValidator;
    private readonly IValidator<PatchSalaryRequest> _patchValidator;
    private static int _nextId = 200;
    private static readonly object _lock = new();

    public EmployeeService(
        IEmployeeRepository repository,
        IPayrollCalculator payrollCalculator,
        IValidator<CreateEmployeeRequest> createValidator,
        IValidator<UpdateEmployeeRequest> updateValidator,
        IValidator<PatchSalaryRequest> patchValidator)
    {
        _repository = repository;
        _payrollCalculator = payrollCalculator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _patchValidator = patchValidator;
    }

    public Task<IEnumerable<EmployeeResponse>> GetAllEmployeesAsync(int? departmentId = null, decimal? minPay = null)
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
        return Task.FromResult(responses);
    }

    public Task<EmployeeResponse> GetEmployeeByIdAsync(int id)
    {
        var employee = _repository.GetById(id)
            ?? throw new NotFoundException("Employee", id);

        return Task.FromResult(MapToResponse(employee));
    }

    public async Task<EmployeeResponse> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Check for duplicate email
        var exists = _repository.GetAll()
            .Any(e => e.Email.Equals(request.Email.Trim(), StringComparison.OrdinalIgnoreCase));

        if (exists)
        {
            throw new ConflictException($"An employee with email '{request.Email}' already exists.");
        }

        Address? address = null;
        if (!string.IsNullOrWhiteSpace(request.Street) &&
            !string.IsNullOrWhiteSpace(request.City) &&
            !string.IsNullOrWhiteSpace(request.State) &&
            !string.IsNullOrWhiteSpace(request.ZipCode))
        {
            address = new Address(request.Street, request.City, request.State, request.ZipCode);
        }

        int id;
        lock (_lock)
        {
            id = ++_nextId;
        }

        Employee employee = request.Type switch
        {
            EmployeeType.FullTime => new FullTimeEmployee(
                id: id,
                name: request.Name.Trim(),
                email: request.Email.Trim().ToLowerInvariant(),
                departmentId: request.DepartmentId,
                annualSalary: request.SalaryOrHourlyRate,
                monthlyBenefitsAllowance: request.MonthlyBenefits,
                address: address
            ),
            EmployeeType.Contract => new ContractEmployee(
                id: id,
                name: request.Name.Trim(),
                email: request.Email.Trim().ToLowerInvariant(),
                departmentId: request.DepartmentId,
                hourlyRate: request.SalaryOrHourlyRate,
                hoursWorkedThisMonth: request.ContractHoursThisMonth,
                contractEndDate: request.ContractEndDate ?? DateTime.UtcNow.AddMonths(6),
                address: address
            ),
            _ => throw new BusinessRuleException($"Unsupported employee type: {request.Type}")
        };

        _repository.Add(employee);
        return MapToResponse(employee);
    }

    public async Task<EmployeeResponse> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request)
    {
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var employee = _repository.GetById(id)
            ?? throw new NotFoundException("Employee", id);

        // Check if email changed to one that belongs to another employee
        var duplicateEmail = _repository.GetAll()
            .Any(e => e.Id != id && e.Email.Equals(request.Email.Trim(), StringComparison.OrdinalIgnoreCase));

        if (duplicateEmail)
        {
            throw new ConflictException($"Email '{request.Email}' is already in use by another employee.");
        }

        employee.UpdateName(request.Name.Trim());
        employee.UpdateEmail(request.Email.Trim().ToLowerInvariant());

        if (!string.IsNullOrWhiteSpace(request.Street) &&
            !string.IsNullOrWhiteSpace(request.City) &&
            !string.IsNullOrWhiteSpace(request.State) &&
            !string.IsNullOrWhiteSpace(request.ZipCode))
        {
            employee.UpdateAddress(new Address(request.Street, request.City, request.State, request.ZipCode));
        }

        _repository.Update(employee);
        return MapToResponse(employee);
    }

    public async Task<EmployeeResponse> UpdateSalaryAsync(int id, PatchSalaryRequest request)
    {
        var validationResult = await _patchValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var employee = _repository.GetById(id)
            ?? throw new NotFoundException("Employee", id);

        if (employee is FullTimeEmployee ft)
        {
            ft.UpdateSalary(request.NewSalaryOrRate);
        }
        else if (employee is ContractEmployee ct)
        {
            ct.UpdateHourlyRate(request.NewSalaryOrRate);
        }

        _repository.Update(employee);
        return MapToResponse(employee);
    }

    public Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = _repository.GetById(id)
            ?? throw new NotFoundException("Employee", id);

        _repository.Delete(id);
        return Task.FromResult(true);
    }

    private static EmployeeResponse MapToResponse(Employee emp)
    {
        return new EmployeeResponse(
            Id: emp.Id,
            Name: emp.Name,
            Email: emp.Email,
            DepartmentId: emp.DepartmentId,
            Type: emp is FullTimeEmployee ? "FullTime" : "Contract",
            MonthlyPay: Math.Round(emp.CalculateMonthlyPay(), 2),
            City: emp.Address?.City,
            IsActive: emp.IsActive,
            CreatedAt: emp.CreatedAt
        );
    }
}
