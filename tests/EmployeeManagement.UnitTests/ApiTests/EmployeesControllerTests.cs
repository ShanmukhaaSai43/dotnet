using EmployeeManagement.API.Controllers;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Application.Validators;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.UnitTests.ApiTests;

public class EmployeesControllerTests
{
    private readonly IEmployeeRepository _repository;
    private readonly IEmployeeService _service;
    private readonly EmployeesController _controller;

    public EmployeesControllerTests()
    {
        _repository = new InMemoryEmployeeRepository();
        _repository.Add(new FullTimeEmployee(1, "Alice Test", "alice@test.com", 1, 120000m));
        _repository.Add(new ContractEmployee(2, "Bob Test", "bob@test.com", 1, hourlyRate: 70m, hoursWorkedThisMonth: 100, contractEndDate: DateTime.UtcNow.AddMonths(3)));

        _service = new EmployeeService(
            _repository,
            new StandardPayrollCalculator(),
            new CreateEmployeeRequestValidator(),
            new UpdateEmployeeRequestValidator(),
            new PatchSalaryRequestValidator()
        );

        _controller = new EmployeesController(_service);
    }

    [Fact]
    public async Task GetAll_ShouldReturn200OkWithEmployees()
    {
        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var employees = Assert.IsAssignableFrom<IEnumerable<EmployeeResponse>>(okResult.Value);
        Assert.Equal(2, employees.Count());
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturn200OkWithEmployee()
    {
        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var employee = Assert.IsType<EmployeeResponse>(okResult.Value);
        Assert.Equal(1, employee.Id);
        Assert.Equal("Alice Test", employee.Name);
    }

    [Fact]
    public async Task GetById_NonExistingId_ShouldThrowNotFoundException()
    {
        // Act & Assert (Thin controller propagates domain/app exceptions to GlobalExceptionHandler)
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetById(999));
    }

    [Fact]
    public async Task Create_ValidFullTimeEmployee_ShouldReturn201CreatedAtAction()
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
        var result = await _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        var response = Assert.IsType<EmployeeResponse>(createdResult.Value);
        Assert.Equal("Charlie New", response.Name);
        Assert.NotNull(_repository.GetById(response.Id));
    }

    [Fact]
    public async Task Create_DuplicateEmail_ShouldThrowConflictException()
    {
        // Arrange - attempting to insert with existing email 'alice@test.com'
        var duplicateRequest = new CreateEmployeeRequest(
            Name: "Alice Duplicate",
            Email: "alice@test.com",
            DepartmentId: 1,
            Type: EmployeeType.FullTime,
            SalaryOrHourlyRate: 95000m
        );

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _controller.Create(duplicateRequest));
    }

    [Fact]
    public async Task Delete_ExistingEmployee_ShouldReturn204NoContent()
    {
        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Null(_repository.GetById(1));
    }

    [Fact]
    public async Task Delete_NonExistingEmployee_ShouldThrowNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.Delete(999));
    }
}
