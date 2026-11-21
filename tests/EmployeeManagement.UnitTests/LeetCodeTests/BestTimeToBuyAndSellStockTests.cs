using EmployeeManagement.Domain.LeetCode;

namespace EmployeeManagement.UnitTests.LeetCodeTests;

public class BestTimeToBuyAndSellStockTests
{
    [Theory]
    [InlineData(new[] { 7, 1, 5, 3, 6, 4 }, 5)] // Buy at 1, sell at 6 => 5
    [InlineData(new[] { 7, 6, 4, 3, 1 }, 0)]     // Decreasing => 0
    [InlineData(new[] { 2, 4, 1 }, 2)]           // Buy at 2, sell at 4 => 2
    [InlineData(new[] { 1, 10 }, 9)]             // Two days => 9
    [InlineData(new[] { 5 }, 0)]                 // Single day => 0
    [InlineData(new int[0], 0)]                  // Empty array => 0
    public void MaxProfit_ShouldReturnMaximumPossibleProfit(int[] prices, int expectedProfit)
    {
        // Arrange & Act
        int actual = BestTimeToBuyAndSellStockSolution.MaxProfit(prices);

        // Assert
        Assert.Equal(expectedProfit, actual);
    }
}
