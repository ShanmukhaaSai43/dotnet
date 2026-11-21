using EmployeeManagement.Domain.Common;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Domain.Entities;

/// <summary>
/// Demonstrates Encapsulation and Composition (A Department contains and manages a list of Employees).
/// Protects invariants: employees cannot be added twice; department budget is protected.
/// </summary>
public class Department : BaseEntity
{
    private readonly List<Employee> _employees = new();

    public string Name { get; private set; }
    public string Code { get; private set; }
    public decimal MonthlyBudget { get; private set; }
    public DepartmentStatus Status { get; private set; }

    // Read-only collection exposed to callers (Encapsulation principle)
    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();

    // Parameterless constructor for EF Core entity materialization
    protected Department() : base()
    {
        Name = string.Empty;
        Code = string.Empty;
    }

    public Department(int id, string name, string code, decimal monthlyBudget) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Department code cannot be empty.", nameof(code));
        if (monthlyBudget <= 0)
            throw new ArgumentOutOfRangeException(nameof(monthlyBudget), "Monthly budget must be positive.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        MonthlyBudget = monthlyBudget;
        Status = DepartmentStatus.Active;
    }

    public void AddEmployee(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        if (_employees.Any(e => e.Id == employee.Id))
            throw new InvalidOperationException($"Employee with ID {employee.Id} already exists in {Name}.");

        employee.DepartmentId = Id;
        _employees.Add(employee);
        MarkUpdated();
    }

    public bool RemoveEmployee(int employeeId)
    {
        var emp = _employees.FirstOrDefault(e => e.Id == employeeId);
        if (emp != null)
        {
            _employees.Remove(emp);
            MarkUpdated();
            return true;
        }
        return false;
    }

    public decimal CalculateTotalMonthlyPayroll()
    {
        return _employees.Where(e => e.IsActive).Sum(e => e.CalculateMonthlyPay());
    }

    public bool IsOverBudget() => CalculateTotalMonthlyPayroll() > MonthlyBudget;
}
