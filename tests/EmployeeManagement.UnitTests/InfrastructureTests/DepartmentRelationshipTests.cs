using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.ValueObjects;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.UnitTests.InfrastructureTests;

public class DepartmentRelationshipTests
{
    [Fact]
    public async Task Department_WithMultipleEmployees_ShouldPersistAndLoadRelationship()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<EmployeeManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var context = new EmployeeManagementDbContext(options))
        {
            var dept = new Department(10, "Design & UX", "DES", 120000m);
            var emp1 = new FullTimeEmployee(100, "Figma Guru", "figma@design.com", 10, 95000m);
            var emp2 = new ContractEmployee(101, "CSS Wizard", "css@design.com", 10, 80m, 140, DateTime.UtcNow.AddMonths(4));

            context.Departments.Add(dept);
            context.Employees.AddRange(emp1, emp2);
            await context.SaveChangesAsync();
        }

        // Act & Assert in a new DbContext instance to verify clean hydration
        using (var context = new EmployeeManagementDbContext(options))
        {
            var loadedDept = await context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == 10);

            Assert.NotNull(loadedDept);
            Assert.Equal("Design & UX", loadedDept.Name);
            Assert.Equal(2, loadedDept.Employees.Count);
            Assert.Contains(loadedDept.Employees, e => e.Name == "Figma Guru");
            Assert.Contains(loadedDept.Employees, e => e.Name == "CSS Wizard");
        }
    }
}
