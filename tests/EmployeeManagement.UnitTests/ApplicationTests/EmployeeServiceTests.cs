using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Application.Validators;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Domain.Services;

namespace EmployeeManagement.UnitTests.ApplicationTests;

public class EmployeeServiceTests
{
    private readonly IEmployeeRepository _repository;
    private readonly IPayrollCalculator _payrollCalculator;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _repository = new InMemoryEmployeeRepository();
        _payrollCalculator = new StandardPayrollCalculator();

        _repository.Add(new FullTimeEmployee(1, "Alice Test", "alice@test.com", 1, 120000m));
        _repository.Add(new ContractEmployee(2, "Bob Test", "bob@test.com", 1, 75m, 160, DateTime.UtcNow.AddMonths(3)));

        _service = new EmployeeService(
            _repository,
            _payrollCalculator,
            new CreateEmployeeRequestValidator(),
            new UpdateEmployeeRequestValidator(),
            new PatchSalaryRequestValidator()
        );
    }

    [Fact]
    public async Task CreateEmployeeAsync_ValidRequest_CreatesAndReturnsResponse()
    {
        // Arrange
        var request = new CreateEmployeeRequest(
            Name: "Charlie New",
            Email: "charlie@new.com",
            DepartmentId: 1,
            Type: EmployeeType.FullTime,
            SalaryOrHourlyRate: 90000m
        );

        // Act
        var response = await _service.CreateEmployeeAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Charlie New", response.Name);
        Assert.Equal("charlie@new.com", response.Email);
        Assert.True(response.Id > 0);
        Assert.NotNull(_repository.GetById(response.Id));
    }

    [Fact]
    public async Task CreateEmployeeAsync_DuplicateEmail_ThrowsConflictException()
    {
        // Arrange (Existing email)
        var request = new CreateEmployeeRequest(
            Name: "Duplicate Person",
            Email: "alice@test.com",
            DepartmentId: 1,
            Type: EmployeeType.FullTime,
            SalaryOrHourlyRate: 80000m
        );

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _service.CreateEmployeeAsync(request));
    }

    [Fact]
    public async Task CreateEmployeeAsync_InvalidRequest_ThrowsValidationException()
    {
        // Arrange (Invalid name and negative department)
        var request = new CreateEmployeeRequest(
            Name: "123!Invalid",
            Email: "not-an-email",
            DepartmentId: -1,
            Type: EmployeeType.FullTime,
            SalaryOrHourlyRate: 5000m
        );

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(() => _service.CreateEmployeeAsync(request));
        Assert.NotEmpty(ex.Errors);
        Assert.True(ex.Errors.ContainsKey(nameof(CreateEmployeeRequest.Name)));
        Assert.True(ex.Errors.ContainsKey(nameof(CreateEmployeeRequest.Email)));
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_NonExisting_ThrowsNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetEmployeeByIdAsync(999));
    }

    [Fact]
    public async Task UpdateSalaryAsync_FullTime_UpdatesSalarySuccessfully()
    {
        // Act
        var response = await _service.UpdateSalaryAsync(1, new PatchSalaryRequest(135000m));

        // Assert
        Assert.NotNull(response);
        var updated = _repository.GetById(1) as FullTimeEmployee;
        Assert.NotNull(updated);
        Assert.Equal(135000m, updated.AnnualSalary);
    }

    [Fact]
    public async Task UpdateSalaryAsync_Contract_UpdatesHourlyRateSuccessfully()
    {
        // Act
        var response = await _service.UpdateSalaryAsync(2, new PatchSalaryRequest(95m));

        // Assert
        Assert.NotNull(response);
        var updated = _repository.GetById(2) as ContractEmployee;
        Assert.NotNull(updated);
        Assert.Equal(95m, updated.HourlyRate);
    }

    [Fact]
    public async Task DeleteEmployeeAsync_Existing_DeletesSuccessfully()
    {
        // Act
        var result = await _service.DeleteEmployeeAsync(1);

        // Assert
        Assert.True(result);
        Assert.Null(_repository.GetById(1));
    }

    [Fact]
    public async Task DeleteEmployeeAsync_NonExisting_ThrowsNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteEmployeeAsync(999));
    }
}
