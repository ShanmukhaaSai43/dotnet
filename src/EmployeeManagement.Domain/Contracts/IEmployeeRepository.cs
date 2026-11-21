using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Domain.Contracts;

/// <summary>
/// Specialized Employee Repository Contract (Interface Segregation Principle).
/// Extends generic repository with employee-specific querying capabilities.
/// </summary>
public interface IEmployeeRepository : IRepository<Employee>
{
    IReadOnlyList<Employee> GetByDepartment(int departmentId);
    IReadOnlyList<T> GetByType<T>() where T : Employee;
    Employee? GetByEmail(string email);
}
