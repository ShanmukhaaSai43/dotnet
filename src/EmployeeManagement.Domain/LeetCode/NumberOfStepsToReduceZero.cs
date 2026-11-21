namespace EmployeeManagement.Domain.LeetCode;

/// <summary>
/// Day 2 LeetCode Problem: 1342. Number of Steps to Reduce a Number to Zero
/// Difficulty: Easy
/// 
/// PROBLEM:
/// Given an integer num, return the number of steps to reduce it to zero.
/// In one step, if the current number is even, you have to divide it by 2,
/// otherwise, you have to subtract 1 from it.
/// 
/// APPROACH:
/// - Approach 1 (Iterative Simulation):
///   While num > 0:
///     If num is even (num % 2 == 0): num /= 2
///     Else: num -= 1
///     steps++
/// 
/// - Approach 2 (Bitwise Insight):
///   - If bit 0 is 1 (odd): 1 step to subtract 1 (turns bit to 0).
///   - If bit 0 is 0 (even): 1 step to shift right (divide by 2).
///   - Number of steps = total set bits + total bit length - 1.
/// 
/// COMPLEXITY:
/// - Time Complexity: O(log N) because in at most two steps the number is halved.
/// - Space Complexity: O(1) auxiliary space (uses only a counter variable).
/// 
/// EDGE CASES TESTED:
/// 1. Zero: num = 0 -> 0 steps (loop does not execute).
/// 2. Power of 2: num = 8 (even all the way: 8 -> 4 -> 2 -> 1 -> 0: 4 steps).
/// 3. Prime number: num = 14 (14 -> 7 -> 6 -> 3 -> 2 -> 1 -> 0: 6 steps).
/// </summary>
public static class NumberOfStepsToReduceZeroSolution
{
    public static int NumberOfSteps(int num)
    {
        if (num <= 0) return 0;

        int steps = 0;
        while (num > 0)
        {
            if ((num & 1) == 0) // Even check using bitwise AND
            {
                num >>= 1; // Divide by 2 using right shift
            }
            else // Odd
            {
                num -= 1;
            }
            steps++;
        }

        return steps;
    }

    public static void RunTests()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    DAY 2 LEETCODE: 1342. Reduce a Number to Zero     ");
        Console.WriteLine("=======================================================\n");

        var testCases = new (int Input, int Expected, string Description)[]
        {
            (14, 6, "Standard: 14 -> 7 -> 6 -> 3 -> 2 -> 1 -> 0"),
            (8, 4, "Power of 2: 8 -> 4 -> 2 -> 1 -> 0"),
            (123, 12, "Odd number: 123"),
            (0, 0, "Edge Case: Zero input"),
            (1, 1, "Edge Case: Single step (1 -> 0)")
        };

        foreach (var (input, expected, description) in testCases)
        {
            int actual = NumberOfSteps(input);
            bool pass = actual == expected;
            Console.WriteLine($"{description}");
            Console.WriteLine($"  Input:    {input}");
            Console.WriteLine($"  Output:   {actual} (Expected: {expected})");
            Console.WriteLine($"  Status:   {(pass ? "PASS" : "FAIL")}\n");
        }
    }
}
