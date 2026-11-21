using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Contracts;

/// <summary>
/// Asynchronous service contract demonstrating Task-based Asynchronous Pattern (TAP)
/// and cooperative cancellation via CancellationToken.
/// </summary>
public interface IAsyncEmployeeService
{
    Task<IReadOnlyList<Employee>> FetchAllEmployeesAsync(CancellationToken cancellationToken = default);
    Task<EmployeeSummaryDto> GetEmployeeSummaryAsync(int id, CancellationToken cancellationToken = default);
    Task<decimal> ComputeTotalPayrollAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> FetchDepartmentHeadcountsConcurrentlyAsync(IEnumerable<int> departmentIds, CancellationToken cancellationToken = default);
}
