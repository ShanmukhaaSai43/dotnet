using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.ValueObjects;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(EmployeeManagementDbContext context)
    {
        // Ensure database tables exist
        await context.Database.EnsureCreatedAsync();

        if (await context.Departments.AnyAsync())
        {
            return; // Already seeded
        }

        var engDept = new Department(0, "Engineering", "ENG", 200000m);
        var hrDept = new Department(0, "Human Resources", "HR", 60000m);
        var salesDept = new Department(0, "Sales & Marketing", "SALES", 90000m);

        context.Departments.AddRange(engDept, hrDept, salesDept);
        await context.SaveChangesAsync();

        var employees = new List<Employee>
        {
            new FullTimeEmployee(
                id: 0,
                name: "Alice Johnson",
                email: "alice.johnson@company.com",
                departmentId: engDept.Id,
                annualSalary: 125000m,
                monthlyBenefitsAllowance: 800m,
                address: new Address("100 Tech Way", "Seattle", "WA", "98101")
            ),
            new ContractEmployee(
                id: 0,
                name: "Bob Martin",
                email: "bob.martin@contractor.com",
                departmentId: engDept.Id,
                hourlyRate: 85m,
                hoursWorkedThisMonth: 160,
                contractEndDate: DateTime.UtcNow.AddMonths(6),
                address: new Address("200 Contract Blvd", "Portland", "OR", "97201")
            ),
            new FullTimeEmployee(
                id: 0,
                name: "Carol White",
                email: "carol.white@company.com",
                departmentId: hrDept.Id,
                annualSalary: 85000m,
                monthlyBenefitsAllowance: 500m,
                address: new Address("300 People St", "Austin", "TX", "73301")
            ),
            new FullTimeEmployee(
                id: 0,
                name: "David Kim",
                email: "david.kim@company.com",
                departmentId: engDept.Id,
                annualSalary: 145000m,
                monthlyBenefitsAllowance: 1000m,
                address: new Address("400 Cloud Ave", "Bellevue", "WA", "98004")
            ),
            new FullTimeEmployee(
                id: 0,
                name: "Elena Rostova",
                email: "elena.rostova@company.com",
                departmentId: salesDept.Id,
                annualSalary: 95000m,
                monthlyBenefitsAllowance: 600m,
                address: new Address("500 Market St", "Chicago", "IL", "60601")
            )
        };

        context.Employees.AddRange(employees);
        await context.SaveChangesAsync();
    }
}
