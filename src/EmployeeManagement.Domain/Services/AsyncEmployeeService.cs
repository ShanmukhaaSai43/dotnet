using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Services;

/// <summary>
/// Implements asynchronous operations with simulated I/O delay,
/// cooperative cancellation, and concurrency via Task.WhenAll.
/// </summary>
public class AsyncEmployeeService : IAsyncEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IEnumerable<Department> _departments;

    public AsyncEmployeeService(IEmployeeRepository repository, IEnumerable<Department> departments)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _departments = departments ?? throw new ArgumentNullException(nameof(departments));
    }

    /// <summary>
    /// Simulates async database/network fetch with cooperative cancellation.
    /// Throws OperationCanceledException if token is cancelled.
    /// </summary>
    public async Task<IReadOnlyList<Employee>> FetchAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        // Simulate non-blocking asynchronous I/O delay (e.g. database query)
        await Task.Delay(200, cancellationToken);

        // Cooperative cancellation check
        cancellationToken.ThrowIfCancellationRequested();

        return _repository.GetAll();
    }

    /// <summary>
    /// Fetches employee summary asynchronously with cancellation check.
    /// </summary>
    public async Task<EmployeeSummaryDto> GetEmployeeSummaryAsync(int id, CancellationToken cancellationToken = default)
    {
        await Task.Delay(100, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        var emp = _repository.GetById(id);
        if (emp == null)
            throw new KeyNotFoundException($"Employee with ID {id} not found.");

        var dept = _departments.FirstOrDefault(d => d.Id == emp.DepartmentId);
        string deptName = dept?.Name ?? "Unassigned";

        return new EmployeeSummaryDto(
            emp.Id,
            emp.Name,
            emp.Email,
            deptName,
            emp.Type,
            emp.CalculateMonthlyPay(),
            emp.Address?.City ?? "N/A"
        );
    }

    /// <summary>
    /// Computes total payroll asynchronously.
    /// </summary>
    public async Task<decimal> ComputeTotalPayrollAsync(CancellationToken cancellationToken = default)
    {
        var employees = await FetchAllEmployeesAsync(cancellationToken);
        return employees.Where(e => e.IsActive).Sum(e => e.CalculateMonthlyPay());
    }

    /// <summary>
    /// Demonstrates Task.WhenAll for fan-out concurrent fetching across departments.
    /// </summary>
    public async Task<Dictionary<string, int>> FetchDepartmentHeadcountsConcurrentlyAsync(
        IEnumerable<int> departmentIds, 
        CancellationToken cancellationToken = default)
    {
        // Create concurrent query tasks for each department
        var tasks = departmentIds.Select(async deptId =>
        {
            await Task.Delay(50, cancellationToken); // Simulates separate remote calls
            cancellationToken.ThrowIfCancellationRequested();

            var dept = _departments.FirstOrDefault(d => d.Id == deptId);
            string name = dept?.Name ?? $"Dept #{deptId}";
            int count = _repository.GetByDepartment(deptId).Count;
            return (Name: name, Count: count);
        });

        // Await all tasks concurrently
        var results = await Task.WhenAll(tasks);

        return results.ToDictionary(r => r.Name, r => r.Count);
    }
}
