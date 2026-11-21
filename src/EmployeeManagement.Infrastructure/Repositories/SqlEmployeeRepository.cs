using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core SQL Server implementation of IEmployeeRepository.
/// Demonstrates AsNoTracking() for read queries and Change Tracker for mutations.
/// </summary>
public class SqlEmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeManagementDbContext _context;

    public SqlEmployeeRepository(EmployeeManagementDbContext context)
    {
        _context = context;
    }

    public Employee? GetById(int id)
    {
        return _context.Employees
            .Include(e => e.Department)
            .FirstOrDefault(e => e.Id == id);
    }

    public Employee? GetByEmail(string email)
    {
        return _context.Employees
            .Include(e => e.Department)
            .FirstOrDefault(e => e.Email.ToLower() == email.Trim().ToLower());
    }

    public IReadOnlyList<Employee> GetAll()
    {
        // AsNoTracking() skips EF Core change tracking overhead for pure read queries
        return _context.Employees
            .Include(e => e.Department)
            .AsNoTracking()
            .ToList();
    }

    public IReadOnlyList<Employee> GetByDepartment(int departmentId)
    {
        return _context.Employees
            .Include(e => e.Department)
            .Where(e => e.DepartmentId == departmentId)
            .AsNoTracking()
            .ToList();
    }

    public IReadOnlyList<T> GetByType<T>() where T : Employee
    {
        return _context.Employees
            .OfType<T>()
            .Include(e => e.Department)
            .AsNoTracking()
            .ToList();
    }

    public void Add(Employee employee)
    {
        _context.Employees.Add(employee);
        _context.SaveChanges();
    }

    public void Update(Employee employee)
    {
        _context.Employees.Update(employee);
        _context.SaveChanges();
    }

    public bool Delete(int id)
    {
        var employee = _context.Employees.Find(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Demonstrates Server-Side SQL Filtering, Sorting, and Pagination with AsNoTracking().
    /// </summary>
    public async Task<PagedResult<Employee>> GetPagedAsync(
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .AsNoTracking()
            .AsQueryable();

        // 1. Filtering by Department
        if (pagination.DepartmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == pagination.DepartmentId.Value);
        }

        // 2. Filtering by Search Term (Name or Email)
        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var search = pagination.SearchTerm.Trim().ToLower();
            query = query.Where(e => e.Name.ToLower().Contains(search) || e.Email.ToLower().Contains(search));
        }

        // 3. Sorting
        query = (pagination.SortBy?.ToLower(), pagination.SortDirection?.ToLower()) switch
        {
            ("email", "desc") => query.OrderByDescending(e => e.Email),
            ("email", _) => query.OrderBy(e => e.Email),
            ("created", "desc") => query.OrderByDescending(e => e.CreatedAt),
            ("created", _) => query.OrderBy(e => e.CreatedAt),
            ("name", "desc") => query.OrderByDescending(e => e.Name),
            _ => query.OrderBy(e => e.Name)
        };

        // 4. Total Count
        int totalCount = await query.CountAsync(cancellationToken);

        // 5. Pagination (Skip and Take)
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Employee>.Create(items, totalCount, pagination.PageNumber, pagination.PageSize);
    }
}
