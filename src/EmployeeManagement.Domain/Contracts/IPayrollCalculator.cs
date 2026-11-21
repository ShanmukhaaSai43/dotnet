using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Contracts;

/// <summary>
/// Abstraction for payroll calculations (Open/Closed Principle & Dependency Inversion).
/// Allows swapping standard tax calculator with international/exempt calculators without modifying entities.
/// </summary>
public interface IPayrollCalculator
{
    decimal CalculateNetPay(Employee employee, out decimal taxDeduction);
    decimal TaxRate { get; }
}
