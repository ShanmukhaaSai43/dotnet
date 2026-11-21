using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;

namespace EmployeeManagement.Domain.Entities;

/// <summary>
/// Full-Time Employee: Inherits from Employee.
/// Demonstrates Polymorphic pay calculation based on fixed annual salary and monthly benefits.
/// </summary>
public class FullTimeEmployee : Employee
{
    public decimal AnnualSalary { get; private set; }
    public decimal MonthlyBenefitsAllowance { get; private set; }
    public string HealthPlanTier { get; private set; }

    public override EmployeeType Type => EmployeeType.FullTime;

    public FullTimeEmployee(
        int id, 
        string name, 
        string email, 
        int departmentId, 
        decimal annualSalary,
        decimal monthlyBenefitsAllowance = 500m,
        string healthPlanTier = "Standard",
        Address? address = null) 
        : base(id, name, email, departmentId, address)
    {
        if (annualSalary < 20000m)
            throw new ArgumentOutOfRangeException(nameof(annualSalary), "Annual salary must be at least $20,000.");

        AnnualSalary = annualSalary;
        MonthlyBenefitsAllowance = monthlyBenefitsAllowance;
        HealthPlanTier = healthPlanTier;
    }

    /// <summary>
    /// Polymorphic Implementation: (AnnualSalary / 12) + Monthly Benefits.
    /// </summary>
    public override decimal CalculateMonthlyPay()
    {
        decimal baseMonthly = AnnualSalary / 12m;
        return decimal.Round(baseMonthly + MonthlyBenefitsAllowance, 2);
    }

    public override string GetRoleSummary() => 
        $"{Name} [Full-Time] | Annual: {AnnualSalary:C0} | Benefits: {MonthlyBenefitsAllowance:C0} | Tier: {HealthPlanTier}";

    public void UpdateSalary(decimal newSalary)
    {
        if (newSalary < 20000m)
            throw new ArgumentOutOfRangeException(nameof(newSalary), "Annual salary must be at least $20,000.");

        AnnualSalary = newSalary;
        MarkUpdated();
    }
}
