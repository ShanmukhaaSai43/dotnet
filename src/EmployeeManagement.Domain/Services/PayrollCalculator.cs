using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Services;

/// <summary>
/// Single Responsibility Principle: This service handles tax logic and net pay calculations.
/// Domain entities (Employee) do NOT know about tax tables or government deductions.
/// </summary>
public class StandardPayrollCalculator : IPayrollCalculator
{
    public decimal TaxRate { get; }

    public StandardPayrollCalculator(decimal taxRate = 0.20m)
    {
        if (taxRate < 0 || taxRate > 0.60m)
            throw new ArgumentOutOfRangeException(nameof(taxRate), "Tax rate must be between 0% and 60%.");

        TaxRate = taxRate;
    }

    public decimal CalculateNetPay(Employee employee, out decimal taxDeduction)
    {
        ArgumentNullException.ThrowIfNull(employee);

        decimal grossPay = employee.CalculateMonthlyPay(); // Polymorphic call!
        taxDeduction = decimal.Round(grossPay * TaxRate, 2);
        return grossPay - taxDeduction;
    }
}
