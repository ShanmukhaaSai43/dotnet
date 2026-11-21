namespace EmployeeManagement.API.Configuration;

/// <summary>
/// Strongly-typed configuration options mapped from appsettings.json
/// via the Options Pattern (IOptions<PayrollSettings>).
/// </summary>
public class PayrollSettings
{
    public const string SectionName = "PayrollSettings";

    public string CompanyName { get; set; } = "Acme Corp";
    public decimal DefaultBonusRate { get; set; } = 0.10m;
    public decimal MinimumAnnualSalary { get; set; } = 30000m;
    public decimal OvertimeMultiplier { get; set; } = 1.50m;
    public bool AutoApproveBenefits { get; set; } = true;
}
