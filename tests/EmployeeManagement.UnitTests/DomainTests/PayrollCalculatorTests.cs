using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Services;

namespace EmployeeManagement.UnitTests.DomainTests;

public class PayrollCalculatorTests
{
    [Fact]
    public void CalculateNetPay_ValidEmployee_ShouldDeductTaxAccurately()
    {
        // Arrange
        IPayrollCalculator calculator = new StandardPayrollCalculator(taxRate: 0.20m);
        var employee = new FullTimeEmployee(1, "David", "david@company.com", 1, annualSalary: 120000m, monthlyBenefitsAllowance: 0m);
        // Monthly gross = $10,000

        // Act
        decimal netPay = calculator.CalculateNetPay(employee, out decimal taxDeducted);

        // Assert
        Assert.Equal(2000m, taxDeducted); // 20% of 10,000
        Assert.Equal(8000m, netPay);       // 10,000 - 2000
    }

    [Fact]
    public void Constructor_NegativeTaxRate_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new StandardPayrollCalculator(taxRate: -0.05m));
    }
}
