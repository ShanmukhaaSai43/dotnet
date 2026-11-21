using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Domain.Services;

/// <summary>
/// Contains 10 Unit-Testable LINQ queries operating over in-memory domain collections.
/// Demonstrates Where, Select, OrderBy, ThenBy, GroupBy, SelectMany, Aggregate, Join, DistinctBy, and Partitioning.
/// </summary>
public class LinqQueryService
{
    /// <summary>
    /// Query 1: Where - Filter active employees earning above a minimum monthly threshold.
    /// Complexity: Time O(N), Space O(K) where K is number of matches.
    /// </summary>
    public List<Employee> GetHighEarners(IEnumerable<Employee> employees, decimal minMonthlyPay)
    {
        return employees
            .Where(e => e.IsActive && e.CalculateMonthlyPay() >= minMonthlyPay)
            .ToList();
    }

    /// <summary>
    /// Query 2: Select & Join - Project employees into lightweight DTOs joining with Department names.
    /// Complexity: Time O(N + M) using hash join, Space O(N).
    /// </summary>
    public List<EmployeeSummaryDto> GetEmployeeSummaries(IEnumerable<Employee> employees, IEnumerable<Department> departments)
    {
        return employees
            .Join(
                departments,
                emp => emp.DepartmentId,
                dept => dept.Id,
                (emp, dept) => new EmployeeSummaryDto(
                    emp.Id,
                    emp.Name,
                    emp.Email,
                    dept.Name,
                    emp.Type,
                    emp.CalculateMonthlyPay(),
                    emp.Address?.City ?? "Not Specified"
                )
            )
            .ToList();
    }

    /// <summary>
    /// Query 3: OrderBy & ThenBy - Multi-level sorting: DepartmentId ascending, then MonthlyPay descending.
    /// Complexity: Time O(N log N), Space O(N).
    /// </summary>
    public List<Employee> SortByDepartmentAndPay(IEnumerable<Employee> employees)
    {
        return employees
            .OrderBy(e => e.DepartmentId)
            .ThenByDescending(e => e.CalculateMonthlyPay())
            .ToList();
    }

    /// <summary>
    /// Query 4: GroupBy - Group employees by Department and calculate aggregation statistics.
    /// Complexity: Time O(N), Space O(D) where D is number of departments.
    /// </summary>
    public List<DepartmentPayrollReportDto> GetDepartmentPayrollReports(
        IEnumerable<Employee> employees, 
        IEnumerable<Department> departments)
    {
        var deptDict = departments.ToDictionary(d => d.Id);

        return employees
            .GroupBy(e => e.DepartmentId)
            .Select(group =>
            {
                deptDict.TryGetValue(group.Key, out var dept);
                string deptName = dept?.Name ?? $"Unknown Dept #{group.Key}";
                decimal budget = dept?.MonthlyBudget ?? 0m;

                var activeList = group.Where(e => e.IsActive).ToList();
                decimal totalPay = activeList.Sum(e => e.CalculateMonthlyPay());
                decimal avgPay = activeList.Any() ? activeList.Average(e => e.CalculateMonthlyPay()) : 0m;
                decimal maxPay = activeList.Any() ? activeList.Max(e => e.CalculateMonthlyPay()) : 0m;

                return new DepartmentPayrollReportDto(
                    DepartmentId: group.Key,
                    DepartmentName: deptName,
                    TotalEmployees: group.Count(),
                    FullTimeCount: group.Count(e => e is FullTimeEmployee),
                    ContractCount: group.Count(e => e is ContractEmployee),
                    TotalMonthlyPayroll: decimal.Round(totalPay, 2),
                    AverageMonthlyPay: decimal.Round(avgPay, 2),
                    HighestMonthlyPay: decimal.Round(maxPay, 2),
                    MonthlyBudget: budget,
                    IsOverBudget: totalPay > budget
                );
            })
            .OrderByDescending(r => r.TotalMonthlyPayroll)
            .ToList();
    }

    /// <summary>
    /// Query 5: GroupBy with Dictionary conversion - Fast O(1) lookup dictionary of Employees by Department.
    /// Complexity: Time O(N), Space O(N).
    /// </summary>
    public Dictionary<int, List<Employee>> GroupEmployeesByDepartmentLookup(IEnumerable<Employee> employees)
    {
        return employees
            .GroupBy(e => e.DepartmentId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// Query 6: SelectMany - Flattens nested collections (extracts all unique skills across employees).
    /// Complexity: Time O(N * S), Space O(U) where S is skills per emp and U is unique skills.
    /// </summary>
    public List<string> GetUniqueSkillsAcrossEmployees(IEnumerable<string[]> skillsPerEmployee)
    {
        return skillsPerEmployee
            .SelectMany(skills => skills)
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s)
            .ToList();
    }

    /// <summary>
    /// Query 7: Aggregate - Custom rolling payroll accumulator with audit logging.
    /// Complexity: Time O(N), Space O(1).
    /// </summary>
    public decimal CalculateTotalPayrollViaAggregate(IEnumerable<Employee> employees)
    {
        return employees
            .Where(e => e.IsActive)
            .Aggregate(0m, (accumulated, nextEmp) => accumulated + nextEmp.CalculateMonthlyPay());
    }

    /// <summary>
    /// Query 8: DistinctBy - Returns one representative employee from each unique City (C# 10+ LINQ).
    /// Complexity: Time O(N), Space O(C) where C is number of unique cities.
    /// </summary>
    public List<Employee> GetFirstEmployeePerCity(IEnumerable<Employee> employees)
    {
        return employees
            .Where(e => e.Address != null && !string.IsNullOrWhiteSpace(e.Address.City))
            .DistinctBy(e => e.Address!.City, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Query 9: Partitioning (Skip & Take) - Pagination query for large employee tables.
    /// Complexity: Time O(Skip + Take), Space O(Take).
    /// </summary>
    public (List<Employee> PageItems, int TotalCount) GetPagedEmployees(
        IEnumerable<Employee> employees, 
        int pageNumber, 
        int pageSize)
    {
        int safePage = Math.Max(1, pageNumber);
        int safeSize = Math.Clamp(pageSize, 1, 100);

        var list = employees.ToList();
        var items = list
            .Skip((safePage - 1) * safeSize)
            .Take(safeSize)
            .ToList();

        return (items, list.Count);
    }

    /// <summary>
    /// Query 10: Modern Pattern Matching with Switch Expressions & Relational Patterns.
    /// Evaluates employee tier based on employment type and pay.
    /// </summary>
    public string ClassifyEmployeeTier(Employee employee)
    {
        decimal pay = employee.CalculateMonthlyPay();

        return (employee, pay) switch
        {
            (FullTimeEmployee ft, >= 12000m) => $"Senior Executive Full-Time ({ft.HealthPlanTier} Plan)",
            (FullTimeEmployee, >= 8000m)     => "Standard Full-Time Professional",
            (FullTimeEmployee, _)            => "Junior Full-Time Associate",
            (ContractEmployee ct, >= 10000m) => $"High-Value Contractor (${ct.HourlyRate}/hr)",
            (ContractEmployee, _)            => "Standard Contractor",
            _                                => "General Contributor"
        };
    }
}
