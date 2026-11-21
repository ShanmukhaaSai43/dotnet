using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Services;

/// <summary>
/// Domain service managing employee bonuses and compensation reviews.
/// Contains the production fix for Defect #BUG-101 (Prorated Bonus Integer Division Bug).
/// </summary>
public class CompensationService
{
    /// <summary>
    /// Calculates prorated annual bonus based on months completed during the fiscal year.
    /// 
    /// DEFECT HISTORY (BUG-101):
    /// Previously, the implementation used integer arithmetic:
    ///   decimal factor = monthsWorked / 12; // BUG: In C#, integer / integer truncates to 0!
    ///   return annualBonus * factor;         // Resulted in $0 bonus for all employees with 1..11 months worked!
    /// 
    /// FIX:
    /// Enforce decimal literal division (12.0m) and bounds validation.
    /// </summary>
    public decimal CalculateProratedBonus(decimal annualBonus, int monthsWorked)
    {
        if (annualBonus < 0)
            throw new ArgumentOutOfRangeException(nameof(annualBonus), "Annual bonus cannot be negative.");

        if (monthsWorked < 0 || monthsWorked > 12)
            throw new ArgumentOutOfRangeException(nameof(monthsWorked), "Months worked must be between 0 and 12.");

        if (monthsWorked == 0 || annualBonus == 0m)
            return 0m;

        // FIXED: Using 12.0m ensures high-precision floating decimal arithmetic without integer truncation
        decimal proratedFactor = monthsWorked / 12.0m;
        return decimal.Round(annualBonus * proratedFactor, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Evaluates performance multiplier for annual bonuses.
    /// Rating: 1 (Unsatisfactory: 0x), 2 (Needs Improvement: 0.5x), 3 (Meets: 1.0x), 4 (Exceeds: 1.25x), 5 (Outstanding: 1.5x).
    /// </summary>
    public decimal CalculatePerformanceBonus(Employee employee, decimal baseBonus, int rating)
    {
        ArgumentNullException.ThrowIfNull(employee);

        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Performance rating must be between 1 and 5.");

        decimal multiplier = rating switch
        {
            5 => 1.50m,
            4 => 1.25m,
            3 => 1.00m,
            2 => 0.50m,
            1 => 0.00m,
            _ => 1.00m
        };

        // Contract employees receive half bonus multiplier per organizational policy
        if (employee is ContractEmployee)
        {
            multiplier *= 0.50m;
        }

        return decimal.Round(baseBonus * multiplier, 2);
    }
}
