using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;

namespace EmployeeManagement.UnitTests.DomainTests;

public class EmployeeTests
{
    [Fact]
    public void FullTimeEmployee_CalculateMonthlyPay_ShouldReturnBasePlusBenefits()
    {
        // Arrange
        var employee = new FullTimeEmployee(
            id: 1,
            name: "John Doe",
            email: "john@company.com",
            departmentId: 10,
            annualSalary: 120000m,
            monthlyBenefitsAllowance: 800m
        );

        // Act
        decimal monthlyPay = employee.CalculateMonthlyPay();

        // Assert
        // ($120,000 / 12) + $800 = $10,800
        Assert.Equal(10800m, monthlyPay);
    }

    [Fact]
    public void ContractEmployee_CalculateMonthlyPay_ShouldReturnRateMultipliedByHours()
    {
        // Arrange
        var contractor = new ContractEmployee(
            id: 2,
            name: "Jane Smith",
            email: "jane@contractor.com",
            departmentId: 10,
            hourlyRate: 75m,
            hoursWorkedThisMonth: 160,
            contractEndDate: DateTime.UtcNow.AddMonths(3)
        );

        // Act
        decimal monthlyPay = contractor.CalculateMonthlyPay();

        // Assert
        // 75 * 160 = $12,000
        Assert.Equal(12000m, monthlyPay);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Employee_InvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new FullTimeEmployee(1, invalidName!, "valid@company.com", 1, 60000m));
    }

    [Theory]
    [InlineData("invalid-email-no-at")]
    [InlineData("")]
    [InlineData(null)]
    public void Employee_InvalidEmail_ShouldThrowArgumentException(string? invalidEmail)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new FullTimeEmployee(1, "Valid Name", invalidEmail!, 1, 60000m));
    }

    [Fact]
    public void Address_RecordValueEquality_ShouldBeEqualWhenPropertiesMatch()
    {
        // Arrange
        var addr1 = new Address("123 Main St", "Dallas", "TX", "75001");
        var addr2 = new Address("123 Main St", "Dallas", "TX", "75001");

        // Act & Assert
        Assert.Equal(addr1, addr2);
        Assert.True(addr1 == addr2);
        Assert.False(ReferenceEquals(addr1, addr2)); // Different heap instances, value equality
    }
}
