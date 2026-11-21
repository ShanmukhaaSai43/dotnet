using System.Text;
using EmployeeManagement.ConsoleApp.Exceptions;
using EmployeeManagement.ConsoleApp.Models;

namespace EmployeeManagement.ConsoleApp.Services;

public class EmployeeService
{
    private readonly List<Employee> _employees = new();
    private int _nextId = 101;

    public EmployeeService()
    {
        SeedInitialData();
    }

    private void SeedInitialData()
    {
        AddEmployee(
            name: "Alice Johnson",
            email: "alice@company.com",
            department: Department.Engineering,
            salary: 85000m,
            bonus: 5000m,
            phone: "555-0101",
            address: new EmployeeAddress("100 Tech Blvd", "Seattle", "WA", "98101"),
            skills: new[] { "C#", ".NET Core", "SQL" }
        );

        AddEmployee(
            name: "Bob Smith",
            email: "bob@company.com",
            department: Department.Finance,
            salary: 72000m,
            bonus: null, // Nullable value type demonstrated
            phone: null, // Nullable reference type demonstrated
            address: new EmployeeAddress("200 Wall St", "New York", "NY", "10005"),
            skills: new[] { "Excel", "Budgeting", "Auditing" }
        );

        AddEmployee(
            name: "Carol Danvers",
            email: "carol@company.com",
            department: Department.Engineering,
            salary: 95000m,
            bonus: 8000m,
            phone: "555-0103",
            address: new EmployeeAddress("300 Cloud Way", "Austin", "TX", "78701"),
            skills: new[] { "C#", "Architecture", "Azure", "Docker" }
        );
    }

    /// <summary>
    /// Demonstrates input validation, exception throwing, and optional/nullable parameters.
    /// </summary>
    public Employee AddEmployee(
        string name,
        string email,
        Department department,
        decimal salary,
        decimal? bonus = null,
        string? phone = null,
        EmployeeAddress address = default,
        string[]? skills = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new EmployeeValidationException("Employee name cannot be null or empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new EmployeeValidationException("A valid email address containing '@' is required.", nameof(email));

        if (salary < 30000m)
            throw new EmployeeValidationException("Salary must be at least 30,000 for standard compensation.", nameof(salary));

        if (bonus.HasValue && bonus.Value < 0)
            throw new EmployeeValidationException("Bonus cannot be negative.", nameof(bonus));

        var employee = new Employee
        {
            Id = _nextId++,
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Department = department,
            Salary = salary,
            Bonus = bonus,
            PhoneNumber = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim(),
            Address = address,
            Skills = skills ?? Array.Empty<string>(),
            HireDate = DateTime.UtcNow,
            IsActive = true
        };

        _employees.Add(employee);
        return employee;
    }

    /// <summary>
    /// Demonstrates 'out' parameter pattern in C#.
    /// </summary>
    public bool TryGetEmployeeById(int id, out Employee? employee)
    {
        foreach (var emp in _employees)
        {
            if (emp.Id == id)
            {
                employee = emp;
                return true;
            }
        }

        employee = null;
        return false;
    }

    public Employee GetEmployeeById(int id)
    {
        if (TryGetEmployeeById(id, out Employee? emp) && emp != null)
        {
            return emp;
        }

        throw new EmployeeNotFoundException(id);
    }

    public IReadOnlyList<Employee> GetAllEmployees() => _employees.AsReadOnly();

    public List<Employee> FindEmployeesByDepartment(Department department)
    {
        var result = new List<Employee>();
        foreach (var emp in _employees)
        {
            if (emp.Department == department && emp.IsActive)
            {
                result.Add(emp);
            }
        }
        return result;
    }

    /// <summary>
    /// Demonstrates passing a value type by 'ref' to modify the caller's value directly.
    /// </summary>
    public void ApplyAnnualRaise(ref decimal salary, decimal percentageRaise)
    {
        if (percentageRaise < 0 || percentageRaise > 100)
            throw new ArgumentOutOfRangeException(nameof(percentageRaise), "Percentage raise must be between 0 and 100.");

        decimal increase = salary * (percentageRaise / 100m);
        salary += increase;
    }

    /// <summary>
    /// Demonstrates StringBuilder efficiency for building tabular reports instead of immutable string concatenation.
    /// </summary>
    public string GenerateFormattedReport()
    {
        var sb = new StringBuilder();
        sb.AppendLine("==========================================================================================");
        sb.AppendLine($"| {"ID",-4} | {"Name",-18} | {"Department",-14} | {"Salary",-11} | {"Bonus",-9} | {"City",-12} |");
        sb.AppendLine("==========================================================================================");

        decimal totalPayroll = 0;
        foreach (var emp in _employees)
        {
            string bonusStr = emp.Bonus.HasValue ? $"{emp.Bonus.Value,9:C0}" : $"{ "N/A",9}";
            sb.AppendLine($"| {emp.Id,-4} | {emp.Name,-18} | {emp.Department,-14} | {emp.Salary,11:C0} | {bonusStr} | {emp.Address.City,-12} |");
            totalPayroll += emp.TotalCompensation;
        }

        sb.AppendLine("==========================================================================================");
        sb.AppendLine($"Total Headcount: {_employees.Count} | Total Payroll Obligation: {totalPayroll:C0}");
        sb.AppendLine("==========================================================================================");

        return sb.ToString();
    }
}
