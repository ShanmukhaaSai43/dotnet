using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;

namespace EmployeeManagement.Domain.Entities;

/// <summary>
/// Contract Employee: Inherits from Employee.
/// Demonstrates Polymorphic pay calculation based on hourly rate and hours worked.
/// </summary>
public class ContractEmployee : Employee
{
    public decimal HourlyRate { get; private set; }
    public int HoursWorkedThisMonth { get; private set; }
    public DateTime ContractEndDate { get; private set; }

    public override EmployeeType Type => EmployeeType.Contract;

    public ContractEmployee(
        int id, 
        string name, 
        string email, 
        int departmentId, 
        decimal hourlyRate,
        int hoursWorkedThisMonth,
        DateTime contractEndDate,
        Address? address = null) 
        : base(id, name, email, departmentId, address)
    {
        if (hourlyRate < 15m)
            throw new ArgumentOutOfRangeException(nameof(hourlyRate), "Hourly rate cannot be less than $15.00.");

        HourlyRate = hourlyRate;
        HoursWorkedThisMonth = hoursWorkedThisMonth;
        ContractEndDate = contractEndDate;
    }

    /// <summary>
    /// Polymorphic Implementation: HourlyRate * HoursWorked (No benefits).
    /// </summary>
    public override decimal CalculateMonthlyPay()
    {
        return decimal.Round(HourlyRate * HoursWorkedThisMonth, 2);
    }

    public void LogHours(int hours)
    {
        if (hours < 0)
            throw new ArgumentOutOfRangeException(nameof(hours), "Hours cannot be negative.");

        HoursWorkedThisMonth += hours;
        MarkUpdated();
    }

    public void UpdateHourlyRate(decimal newRate)
    {
        if (newRate < 15m)
            throw new ArgumentOutOfRangeException(nameof(newRate), "Hourly rate cannot be less than $15.00.");

        HourlyRate = newRate;
        MarkUpdated();
    }

    public override string GetRoleSummary() => 
        $"{Name} [Contract] | Rate: {HourlyRate:C}/hr | Logged: {HoursWorkedThisMonth}h | Ends: {ContractEndDate:yyyy-MM-dd}";
}
