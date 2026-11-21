using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Domain.Services;
using EmployeeManagement.Domain.ValueObjects;

namespace EmployeeManagement.ConsoleApp.Demos;

/// <summary>
/// Day 2 Demonstrations:
/// 1. Polymorphism (FullTime vs Contract Employee payroll)
/// 2. Encapsulation & Invariant Protection (Department budget & duplicate checking)
/// 3. Records vs Classes (Value Equality & 'with' expressions)
/// 4. SOLID & Dependency Inversion (Repository & PayrollCalculator)
/// </summary>
public static class OopAndSolidDemo
{
    public static void RunAllDemos()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("   DAY 2 DEMO: OOP PILLARS & SOLID PRINCIPLES          ");
        Console.WriteLine("=======================================================\n");

        DemoPolymorphism();
        DemoEncapsulationAndDepartment();
        DemoRecordValueEquality();
        DemoDependencyInversionAndRepository();
    }

    private static void DemoPolymorphism()
    {
        Console.WriteLine("--- 1. Polymorphism in Action (Liskov Substitution) ---");
        
        // Polymorphic collection of Employee base class
        var employees = new List<Employee>
        {
            new FullTimeEmployee(1, "Sarah Connor", "sarah@cyberdyne.com", 10, annualSalary: 120000m, monthlyBenefitsAllowance: 800m),
            new ContractEmployee(2, "Kyle Reese", "kyle@resistance.org", 10, hourlyRate: 75m, hoursWorkedThisMonth: 160, contractEndDate: DateTime.UtcNow.AddMonths(6)),
            new ContractEmployee(3, "John Connor", "john@resistance.org", 10, hourlyRate: 50m, hoursWorkedThisMonth: 80, contractEndDate: DateTime.UtcNow.AddMonths(3))
        };

        // Notice: The caller does NOT need if/else or switch on EmployeeType!
        foreach (var emp in employees)
        {
            decimal monthlyPay = emp.CalculateMonthlyPay(); // Polymorphic method call
            Console.WriteLine($"[Polymorphic Call] {emp.GetRoleSummary()}");
            Console.WriteLine($"  => Monthly Pay: {monthlyPay:C2}\n");
        }
    }

    private static void DemoEncapsulationAndDepartment()
    {
        Console.WriteLine("--- 2. Encapsulation & Invariant Protection (Department) ---");
        var engineering = new Department(10, "Software Engineering", "ENG", monthlyBudget: 25000m);

        var emp1 = new FullTimeEmployee(101, "Grace Hopper", "grace@navy.mil", 10, annualSalary: 150000m, monthlyBenefitsAllowance: 1000m);
        var emp2 = new ContractEmployee(102, "Alan Turing", "alan@bletchley.uk", 10, hourlyRate: 90m, hoursWorkedThisMonth: 120, contractEndDate: DateTime.UtcNow.AddYears(1));

        engineering.AddEmployee(emp1);
        engineering.AddEmployee(emp2);

        Console.WriteLine($"Department: {engineering.Name} (Code: {engineering.Code})");
        Console.WriteLine($"Monthly Budget:    {engineering.MonthlyBudget:C2}");
        Console.WriteLine($"Total Monthly Pay: {engineering.CalculateTotalMonthlyPayroll():C2}");
        Console.WriteLine($"Is Over Budget?    {engineering.IsOverBudget()}");

        // Attempt duplicate addition - Encapsulation protects invariant
        try
        {
            Console.WriteLine("\nAttempting to add duplicate employee with ID 101...");
            engineering.AddEmployee(emp1);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[Guarded by Encapsulation]: {ex.Message}");
        }
        Console.WriteLine();
    }

    private static void DemoRecordValueEquality()
    {
        Console.WriteLine("--- 3. Modern C#: Records & Value Equality ---");

        var addr1 = new Address("742 Evergreen Terrace", "Springfield", "OR", "97477");
        var addr2 = new Address("742 Evergreen Terrace", "Springfield", "OR", "97477");

        Console.WriteLine($"Address 1: {addr1}");
        Console.WriteLine($"Address 2: {addr2}");
        Console.WriteLine($"Are references equal?     {ReferenceEquals(addr1, addr2)} (False - distinct objects on Heap)");
        Console.WriteLine($"Are values equal (==)?    {addr1 == addr2} (True - Records have built-in value equality!)");

        // Non-destructive mutation using 'with' keyword
        var addr3 = addr1 with { Street = "744 Evergreen Terrace" };
        Console.WriteLine($"Address 3 (via 'with'):   {addr3}");
        Console.WriteLine($"Is Address 1 unchanged?   {addr1.Street == "742 Evergreen Terrace"} (True - Immutable)");
        Console.WriteLine();
    }

    private static void DemoDependencyInversionAndRepository()
    {
        Console.WriteLine("--- 4. Interface-Driven Design (Repository & PayrollCalculator) ---");

        IEmployeeRepository repository = new InMemoryEmployeeRepository();
        IPayrollCalculator calculator = new StandardPayrollCalculator(taxRate: 0.22m);

        var emp = new FullTimeEmployee(201, "Ada Lovelace", "ada@computing.org", 10, annualSalary: 140000m);
        repository.Add(emp);

        var retrieved = repository.GetById(201);
        if (retrieved != null)
        {
            decimal netPay = calculator.CalculateNetPay(retrieved, out decimal taxDeduction);
            Console.WriteLine($"Retrieved from IEmployeeRepository: {retrieved.Name}");
            Console.WriteLine($"Gross Pay:      {retrieved.CalculateMonthlyPay():C2}");
            Console.WriteLine($"Tax Rate:       {calculator.TaxRate:P0}");
            Console.WriteLine($"Tax Deducted:   {taxDeduction:C2}");
            Console.WriteLine($"Net Pay Out:    {netPay:C2}");
        }
        Console.WriteLine();
    }
}
