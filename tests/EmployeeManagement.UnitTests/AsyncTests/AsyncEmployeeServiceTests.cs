using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Domain.Services;

namespace EmployeeManagement.UnitTests.AsyncTests;

public class AsyncEmployeeServiceTests
{
    private readonly IEmployeeRepository _repository;
    private readonly List<Department> _departments;
    private readonly AsyncEmployeeService _service;

    public AsyncEmployeeServiceTests()
    {
        _repository = new InMemoryEmployeeRepository();
        _departments = new List<Department>
        {
            new Department(1, "Engineering", "ENG", 50000m),
            new Department(2, "Marketing", "MKT", 30000m)
        };

        _repository.Add(new FullTimeEmployee(1, "Alice", "alice@c.com", 1, 120000m));
        _repository.Add(new ContractEmployee(2, "Bob", "bob@c.com", 2, hourlyRate: 60m, hoursWorkedThisMonth: 100, contractEndDate: DateTime.UtcNow.AddMonths(3)));

        _service = new AsyncEmployeeService(_repository, _departments);
    }

    [Fact]
    public async Task FetchAllEmployeesAsync_WhenNotCancelled_ShouldReturnAllEmployees()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var result = await _service.FetchAllEmployeesAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task FetchAllEmployeesAsync_WhenCancellationRequested_ShouldThrowOperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Immediately signal cancellation!

        // Act & Assert (AAA pattern)
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await _service.FetchAllEmployeesAsync(cts.Token);
        });
    }

    [Fact]
    public async Task FetchDepartmentHeadcountsConcurrentlyAsync_UsingTaskWhenAll_ShouldReturnAllCounts()
    {
        // Arrange
        var deptIds = new[] { 1, 2 };

        // Act
        var headcounts = await _service.FetchDepartmentHeadcountsConcurrentlyAsync(deptIds);

        // Assert
        Assert.Equal(2, headcounts.Count);
        Assert.Equal(1, headcounts["Engineering"]);
        Assert.Equal(1, headcounts["Marketing"]);
    }
}
