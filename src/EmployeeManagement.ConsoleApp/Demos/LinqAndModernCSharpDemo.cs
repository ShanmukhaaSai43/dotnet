using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Events;
using EmployeeManagement.Domain.Extensions;
using EmployeeManagement.Domain.Services;
using EmployeeManagement.Domain.ValueObjects;

namespace EmployeeManagement.ConsoleApp.Demos;

/// <summary>
/// Day 3 Demonstrations:
/// 1. 10 LINQ Operations (Where, Select, GroupBy, OrderBy, Aggregate, Join, SelectMany, DistinctBy, Skip/Take)
/// 2. Deferred Execution vs Immediate Execution
/// 3. Modern C# Pattern Matching & Switch Expressions
/// 4. Delegates and Events in Action
/// </summary>
public static class LinqAndModernCSharpDemo
{
    public static void RunAllDemos()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    DAY 3 DEMO: MODERN C# & LINQ DATA TRANSFORMATIONS  ");
        Console.WriteLine("=======================================================\n");

        var (departments, employees) = SeedData();
        var linqService = new LinqQueryService();

        DemoLinqQueries(linqService, departments, employees);
        DemoDeferredExecution(employees);
        DemoPatternMatching(linqService, employees);
        DemoDelegatesAndEvents();
    }

    private static (List<Department> Departments, List<Employee> Employees) SeedData()
    {
        var d1 = new Department(1, "Engineering", "ENG", monthlyBudget: 40000m);
        var d2 = new Department(2, "Product & Design", "PRD", monthlyBudget: 22000m);
        var d3 = new Department(3, "Sales & Marketing", "MKT", monthlyBudget: 18000m);

        var departments = new List<Department> { d1, d2, d3 };

        var employees = new List<Employee>
        {
            new FullTimeEmployee(101, "Alice Cooper", "alice@company.com", 1, annualSalary: 132000m, monthlyBenefitsAllowance: 800m, address: new Address("100 Tech Way", "Seattle", "WA", "98101")),
            new FullTimeEmployee(102, "Bob Martin", "bob@company.com", 1, annualSalary: 96000m, monthlyBenefitsAllowance: 600m, address: new Address("200 Pine St", "Seattle", "WA", "98101")),
            new ContractEmployee(103, "Charlie Day", "charlie@contract.com", 1, hourlyRate: 85m, hoursWorkedThisMonth: 160, contractEndDate: DateTime.UtcNow.AddMonths(4), address: new Address("300 Oak Ave", "Portland", "OR", "97201")),
            new FullTimeEmployee(104, "Diana Prince", "diana@company.com", 2, annualSalary: 114000m, monthlyBenefitsAllowance: 750m, address: new Address("400 Bay St", "San Francisco", "CA", "94101")),
            new ContractEmployee(105, "Evan Wright", "evan@contract.com", 2, hourlyRate: 60m, hoursWorkedThisMonth: 120, contractEndDate: DateTime.UtcNow.AddMonths(2), address: new Address("500 Market St", "San Francisco", "CA", "94105")),
            new FullTimeEmployee(106, "Fiona Gallagher", "fiona@company.com", 3, annualSalary: 72000m, monthlyBenefitsAllowance: 500m, address: new Address("600 Michigan Ave", "Chicago", "IL", "60601")),
            new ContractEmployee(107, "George Clark", "george@contract.com", 3, hourlyRate: 55m, hoursWorkedThisMonth: 150, contractEndDate: DateTime.UtcNow.AddMonths(6), address: new Address("700 Clark St", "Chicago", "IL", "60602"))
        };

        // Link employees to departments
        foreach (var emp in employees)
        {
            var dept = departments.First(d => d.Id == emp.DepartmentId);
            dept.AddEmployee(emp);
        }

        return (departments, employees);
    }

    private static void DemoLinqQueries(LinqQueryService linq, List<Department> departments, List<Employee> employees)
    {
        Console.WriteLine("--- 1. Top LINQ Queries Showcase ---");

        // Query 1: High Earners
        var highEarners = linq.GetHighEarners(employees, minMonthlyPay: 10000m);
        Console.WriteLine($"[Query 1: Where] Employees earning >= $10,000/mo ({highEarners.Count} found):");
        foreach (var e in highEarners)
            Console.WriteLine($"  - {e.Name} ({e.Type}): {e.CalculateMonthlyPay():C2}/mo");

        // Query 2: Join & Project DTOs
        var summaries = linq.GetEmployeeSummaries(employees, departments);
        Console.WriteLine($"\n[Query 2: Join & Select] Projected DTO Summaries (First 3):");
        foreach (var s in summaries.Take(3))
            Console.WriteLine($"  - {s.Name} | {s.DepartmentName} | {s.MonthlyPay:C2} | City: {s.City}");

        // Query 3: Multi-column OrderBy
        var sorted = linq.SortByDepartmentAndPay(employees);
        Console.WriteLine($"\n[Query 3: OrderBy & ThenBy] Sorted by Dept Asc, Pay Desc (First 3):");
        foreach (var s in sorted.Take(3))
            Console.WriteLine($"  - Dept #{s.DepartmentId} | {s.Name,-15} | Pay: {s.CalculateMonthlyPay():C2}");

        // Query 4: GroupBy Department Summary Report
        var reports = linq.GetDepartmentPayrollReports(employees, departments);
        Console.WriteLine($"\n[Query 4: GroupBy] Department Payroll Aggregations:");
        foreach (var r in reports)
        {
            string budgetStatus = r.IsOverBudget ? "OVER BUDGET" : "OK";
            Console.WriteLine($"  - {r.DepartmentName,-18} | Staff: {r.TotalEmployees} (FT:{r.FullTimeCount}, C:{r.ContractCount}) | Total: {r.TotalMonthlyPayroll,10:C2} | Avg: {r.AverageMonthlyPay,9:C2} | Budget: {r.MonthlyBudget:C2} [{budgetStatus}]");
        }

        // Query 6: SelectMany for distinct skills
        var skillsData = new List<string[]>
        {
            new[] { "C#", ".NET Core", "SQL" },
            new[] { "C#", "Azure", "Docker" },
            new[] { "Figma", "UI/UX", "CSS" },
            new[] { "SEO", "Analytics", "SQL" }
        };
        var uniqueSkills = linq.GetUniqueSkillsAcrossEmployees(skillsData);
        Console.WriteLine($"\n[Query 6: SelectMany] Unique skills across teams ({uniqueSkills.Count}):");
        Console.WriteLine($"  {string.Join(", ", uniqueSkills)}");

        // Query 7: Aggregate total payroll
        decimal totalPayroll = linq.CalculateTotalPayrollViaAggregate(employees);
        Console.WriteLine($"\n[Query 7: Aggregate] Organization Total Monthly Payroll: {totalPayroll:C2}");

        // Query 8: DistinctBy city
        var distinctCities = linq.GetFirstEmployeePerCity(employees);
        Console.WriteLine($"\n[Query 8: DistinctBy] Representative Employees from unique cities ({distinctCities.Count}):");
        foreach (var emp in distinctCities)
            Console.WriteLine($"  - {emp.Address?.City}: {emp.Name}");

        // Query 9: Pagination
        var (page1, total) = linq.GetPagedEmployees(employees, pageNumber: 1, pageSize: 3);
        Console.WriteLine($"\n[Query 9: Skip & Take] Page 1 of Employees ({page1.Count} of {total} total):");
        foreach (var p in page1)
            Console.WriteLine($"  - [ID: {p.Id}] {p.Name}");

        Console.WriteLine();
    }

    private static void DemoDeferredExecution(List<Employee> employees)
    {
        Console.WriteLine("--- 2. Deferred Execution in Action (Lazy Evaluation) ---");

        int inspectionsCount = 0;
        // The query is DEFINED here, but NOTHING executes yet!
        var lazyQuery = employees.WhereLazy(
            e => e.CalculateMonthlyPay() > 9000m,
            e => inspectionsCount++
        );

        Console.WriteLine($"Query created. Inspections count BEFORE enumeration: {inspectionsCount} (Proves zero work done yet!)");

        Console.WriteLine("Enumerating first 2 items via 'Take(2)'...");
        foreach (var emp in lazyQuery.Take(2))
        {
            Console.WriteLine($"  Iterated match: {emp.Name}");
        }

        Console.WriteLine($"Inspections count AFTER Take(2): {inspectionsCount} (Only inspected until 2 matches were found, not the entire list!)");
        Console.WriteLine();
    }

    private static void DemoPatternMatching(LinqQueryService linq, List<Employee> employees)
    {
        Console.WriteLine("--- 3. Modern Pattern Matching (Switch Expressions) ---");
        foreach (var emp in employees.Take(4))
        {
            string tier = linq.ClassifyEmployeeTier(emp);
            Console.WriteLine($"  {emp.Name,-15} ({emp.Type,-8}) -> {tier}");
        }
        Console.WriteLine();
    }

    private static void DemoDelegatesAndEvents()
    {
        Console.WriteLine("--- 4. Custom Delegates & Event Subscriptions ---");

        var notifier = new EmployeeNotifier();

        // Subscribe to event using lambda
        notifier.EmployeeHired += (sender, args) =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[EVENT RECEIVED] Welcome {args.EmployeeName}! Hired to '{args.DepartmentName}' on {args.HiredAt:yyyy-MM-dd HH:mm:ss} UTC.");
            Console.ResetColor();
        };

        // Fire event
        notifier.OnEmployeeHired(108, "Grace Kelly", "Product & Design");
        Console.WriteLine();
    }
}
