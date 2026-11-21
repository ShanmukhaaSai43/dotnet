using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Repositories;

/// <summary>
/// In-memory implementation of IEmployeeRepository.
/// Demonstrates separation of persistence mechanism from domain business rules.
/// </summary>
public class InMemoryEmployeeRepository : IEmployeeRepository
{
    private readonly List<Employee> _storage = new();

    public Employee? GetById(int id)
    {
        return _storage.FirstOrDefault(e => e.Id == id);
    }

    public IReadOnlyList<Employee> GetAll() => _storage.AsReadOnly();

    public void Add(Employee entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (_storage.Any(e => e.Id == entity.Id))
            throw new InvalidOperationException($"Employee with ID {entity.Id} already exists.");

        _storage.Add(entity);
    }

    public void Update(Employee entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existingIndex = _storage.FindIndex(e => e.Id == entity.Id);
        if (existingIndex < 0)
            throw new KeyNotFoundException($"Employee with ID {entity.Id} not found.");

        entity.MarkUpdated();
        _storage[existingIndex] = entity;
    }

    public bool Delete(int id)
    {
        var existing = GetById(id);
        if (existing != null)
        {
            return _storage.Remove(existing);
        }
        return false;
    }

    public IReadOnlyList<Employee> GetByDepartment(int departmentId)
    {
        return _storage.Where(e => e.DepartmentId == departmentId && e.IsActive).ToList();
    }

    public IReadOnlyList<T> GetByType<T>() where T : Employee
    {
        return _storage.OfType<T>().Where(e => e.IsActive).ToList();
    }

    public Employee? GetByEmail(string email)
    {
        return _storage.FirstOrDefault(e => string.Equals(e.Email, email, StringComparison.OrdinalIgnoreCase));
    }
}
