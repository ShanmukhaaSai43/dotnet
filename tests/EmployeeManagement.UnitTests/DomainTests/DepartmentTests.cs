using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.UnitTests.DomainTests;

public class DepartmentTests
{
    [Fact]
    public void AddEmployee_ValidEmployee_ShouldAddToCollection()
    {
        // Arrange
        var department = new Department(1, "Engineering", "ENG", monthlyBudget: 50000m);
        var employee = new FullTimeEmployee(101, "Alice", "alice@company.com", 1, 96000m);

        // Act
        department.AddEmployee(employee);

        // Assert
        Assert.Single(department.Employees);
        Assert.Contains(employee, department.Employees);
        Assert.Equal(1, employee.DepartmentId);
    }

    [Fact]
    public void AddEmployee_DuplicateEmployeeId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var department = new Department(1, "Engineering", "ENG", monthlyBudget: 50000m);
        var emp1 = new FullTimeEmployee(101, "Alice", "alice@company.com", 1, 96000m);
        var emp2 = new FullTimeEmployee(101, "Duplicate Alice", "alice2@company.com", 1, 96000m);
        department.AddEmployee(emp1);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => department.AddEmployee(emp2));
        Assert.Contains("already exists", ex.Message);
    }

    [Fact]
    public void IsOverBudget_WhenTotalPayrollExceedsBudget_ShouldReturnTrue()
    {
        // Arrange
        var department = new Department(1, "Marketing", "MKT", monthlyBudget: 10000m);
        // Annual 120,000 => $10,000/mo + $500 benefits = $10,500/mo > $10,000 budget
        var highEarner = new FullTimeEmployee(101, "Bob", "bob@company.com", 1, annualSalary: 120000m, monthlyBenefitsAllowance: 500m);
        department.AddEmployee(highEarner);

        // Act
        bool overBudget = department.IsOverBudget();

        // Assert
        Assert.True(overBudget);
    }

    [Fact]
    public void RemoveEmployee_ExistingEmployee_ShouldReturnTrueAndRemove()
    {
        // Arrange
        var department = new Department(1, "Finance", "FIN", monthlyBudget: 30000m);
        var emp = new FullTimeEmployee(101, "Charlie", "charlie@company.com", 1, 60000m);
        department.AddEmployee(emp);

        // Act
        bool removed = department.RemoveEmployee(101);

        // Assert
        Assert.True(removed);
        Assert.Empty(department.Employees);
    }
}
