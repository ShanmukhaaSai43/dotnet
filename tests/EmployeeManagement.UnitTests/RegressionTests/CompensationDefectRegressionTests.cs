using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Services;

namespace EmployeeManagement.UnitTests.RegressionTests;

/// <summary>
/// Gate 1 Requirement: Tests for Normal, Boundary, and Invalid inputs,
/// plus a dedicated Regression Test proving that defect #BUG-101 is permanently resolved.
/// </summary>
public class CompensationDefectRegressionTests
{
    private readonly CompensationService _service = new();

    #region REGRESSION TEST (Proves BUG-101 is Fixed)

    /// <summary>
    /// REGRESSION TEST: BUG-101
    /// Defect: Prior implementation used integer division (monthsWorked / 12), which evaluated 
    /// 6 / 12 as 0, resulting in $0 bonus instead of $6,000 for mid-year hires.
    /// This regression test guarantees the defect is fixed and will never reoccur.
    /// </summary>
    [Fact]
    public void CalculateProratedBonus_MidYearHire_ShouldNotTruncateToZero_RegressionTest()
    {
        // Arrange
        decimal annualBonus = 12000m;
        int monthsWorked = 6; // Exactly half the year

        // Act
        decimal result = _service.CalculateProratedBonus(annualBonus, monthsWorked);

        // Assert
        // In the buggy code: 6 / 12 = 0 => 12000 * 0 = 0 (FAILED)
        // In the fixed code: 6 / 12.0m = 0.5m => 12000 * 0.5 = 6000 (PASSED)
        Assert.Equal(6000.00m, result);
        Assert.NotEqual(0m, result);
    }

    #endregion

    #region NORMAL INPUT TESTS

    [Theory]
    [InlineData(12000, 3, 3000.00)]  // 1/4 year
    [InlineData(12000, 4, 4000.00)]  // 1/3 year
    [InlineData(12000, 8, 8000.00)]  // 2/3 year
    [InlineData(12000, 9, 9000.00)]  // 3/4 year
    public void CalculateProratedBonus_NormalMonths_ShouldCalculateProportionately(
        decimal annualBonus, int monthsWorked, decimal expectedBonus)
    {
        // Arrange & Act
        decimal actual = _service.CalculateProratedBonus(annualBonus, monthsWorked);

        // Assert
        Assert.Equal(expectedBonus, actual);
    }

    [Fact]
    public void CalculatePerformanceBonus_FullTimeEmployee_OutstandingRating_ShouldApply150Percent()
    {
        // Arrange
        var emp = new FullTimeEmployee(1, "Top Performer", "top@c.com", 1, 120000m);
        decimal baseBonus = 10000m;
        int rating = 5; // Outstanding => 1.5x multiplier

        // Act
        decimal actual = _service.CalculatePerformanceBonus(emp, baseBonus, rating);

        // Assert
        Assert.Equal(15000.00m, actual);
    }

    #endregion

    #region BOUNDARY INPUT TESTS

    [Fact]
    public void CalculateProratedBonus_ZeroMonthsWorked_BoundaryLower_ShouldReturnZero()
    {
        // Arrange & Act
        decimal result = _service.CalculateProratedBonus(12000m, monthsWorked: 0);

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateProratedBonus_TwelveMonthsWorked_BoundaryUpper_ShouldReturnFullBonus()
    {
        // Arrange & Act
        decimal result = _service.CalculateProratedBonus(12000m, monthsWorked: 12);

        // Assert
        Assert.Equal(12000.00m, result);
    }

    [Fact]
    public void CalculateProratedBonus_OneMonthWorked_BoundaryLowerNonZero_ShouldReturnOneTwelfth()
    {
        // Arrange (10,000 / 12 = 833.33)
        decimal result = _service.CalculateProratedBonus(10000m, monthsWorked: 1);

        // Assert
        Assert.Equal(833.33m, result);
    }

    #endregion

    #region INVALID INPUT TESTS

    [Theory]
    [InlineData(-1)]
    [InlineData(-12)]
    [InlineData(13)]
    [InlineData(100)]
    public void CalculateProratedBonus_InvalidMonthsWorked_ShouldThrowArgumentOutOfRangeException(int invalidMonths)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _service.CalculateProratedBonus(10000m, invalidMonths));
    }

    [Fact]
    public void CalculateProratedBonus_NegativeBonus_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _service.CalculateProratedBonus(-500m, 6));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void CalculatePerformanceBonus_InvalidRating_ShouldThrowArgumentOutOfRangeException(int invalidRating)
    {
        // Arrange
        var emp = new FullTimeEmployee(1, "Test", "t@c.com", 1, 60000m);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _service.CalculatePerformanceBonus(emp, 5000m, invalidRating));
    }

    #endregion
}
