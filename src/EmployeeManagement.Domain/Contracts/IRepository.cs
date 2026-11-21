using EmployeeManagement.Domain.Common;

namespace EmployeeManagement.Domain.Contracts;

/// <summary>
/// Generic Repository Interface (Interface Segregation & Dependency Inversion).
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    T? GetById(int id);
    IReadOnlyList<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    bool Delete(int id);
}
