namespace EmployeeManagement.Domain.LeetCode;

/// <summary>
/// Day 4 LeetCode Problem: 121. Best Time to Buy and Sell Stock
/// Difficulty: Easy
/// 
/// PROBLEM:
/// You are given an array prices where prices[i] is the price of a given stock on the ith day.
/// You want to maximize your profit by choosing a single day to buy one stock and choosing
/// a different day in the future to sell that stock.
/// Return the maximum profit you can achieve from this transaction. If you cannot achieve any profit, return 0.
/// 
/// APPROACH (One-Pass Greedy / Sliding Minimum):
/// - Maintain 'minPrice' initialized to int.MaxValue.
/// - Maintain 'maxProfit' initialized to 0.
/// - For each price in prices:
///     If price < minPrice: update minPrice = price
///     Else: potentialProfit = price - minPrice
///           if potentialProfit > maxProfit: maxProfit = potentialProfit
/// - Return maxProfit.
/// 
/// COMPLEXITY:
/// - Time Complexity: O(N) — single pass through the array.
/// - Space Complexity: O(1) — constant extra space.
/// 
/// EDGE CASES TESTED:
/// 1. Decreasing prices: [7, 6, 4, 3, 1] -> profit = 0 (cannot profit).
/// 2. General case: [7, 1, 5, 3, 6, 4] -> buy at 1, sell at 6 = 5.
/// 3. Single day: [5] -> profit = 0.
/// 4. Two days (profit): [1, 10] -> profit = 9.
/// </summary>
public static class BestTimeToBuyAndSellStockSolution
{
    public static int MaxProfit(int[] prices)
    {
        if (prices == null || prices.Length <= 1)
            return 0;

        int minPrice = int.MaxValue;
        int maxProfit = 0;

        for (int i = 0; i < prices.Length; i++)
        {
            int currentPrice = prices[i];
            if (currentPrice < minPrice)
            {
                minPrice = currentPrice;
            }
            else
            {
                int profit = currentPrice - minPrice;
                if (profit > maxProfit)
                {
                    maxProfit = profit;
                }
            }
        }

        return maxProfit;
    }

    public static void RunTests()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    DAY 4 LEETCODE: 121. Best Time to Buy and Sell     ");
        Console.WriteLine("=======================================================\n");

        var testCases = new (int[] Prices, int Expected, string Description)[]
        {
            (new[] { 7, 1, 5, 3, 6, 4 }, 5, "Standard: Buy at 1, sell at 6 (profit = 5)"),
            (new[] { 7, 6, 4, 3, 1 }, 0, "Decreasing prices: No profit possible (profit = 0)"),
            (new[] { 2, 4, 1 }, 2, "Buy at 2, sell at 4 (profit = 2)"),
            (new[] { 5 }, 0, "Single element: profit = 0"),
            (new[] { 1, 10 }, 9, "Two elements: Buy at 1, sell at 10 (profit = 9)")
        };

        foreach (var (prices, expected, description) in testCases)
        {
            int actual = MaxProfit(prices);
            bool pass = actual == expected;
            Console.WriteLine($"{description}");
            Console.WriteLine($"  Prices:   [{string.Join(", ", prices)}]");
            Console.WriteLine($"  Profit:   {actual} (Expected: {expected})");
            Console.WriteLine($"  Status:   {(pass ? "PASS" : "FAIL")}\n");
        }
    }
}
