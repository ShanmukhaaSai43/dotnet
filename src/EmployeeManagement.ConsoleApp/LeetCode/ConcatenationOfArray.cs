namespace EmployeeManagement.ConsoleApp.LeetCode;

/// <summary>
/// Day 1 LeetCode Problem: 1929. Concatenation of Array
/// Difficulty: Easy
/// 
/// PROBLEM:
/// Given an integer array nums of length n, you want to create an array ans of length 2n where
/// ans[i] == nums[i] and ans[i + n] == nums[i] for 0 <= i < n (0-indexed).
/// Specifically, ans is the concatenation of two nums arrays.
/// Return the array ans.
/// 
/// APPROACH:
/// 1. Let n = nums.Length. Allocate a new result array 'ans' of size 2 * n.
/// 2. Loop i from 0 to n - 1:
///      ans[i] = nums[i]
///      ans[i + n] = nums[i]
/// 3. Return ans.
/// 
/// COMPLEXITY:
/// - Time Complexity: O(N) because we iterate through the input array exactly once.
/// - Space Complexity: O(N) auxiliary space for the output array of length 2N (O(1) extra working memory).
/// 
/// EDGE CASES TESTED:
/// 1. Single-element array: [1] -> [1, 1]
/// 2. Array with duplicate elements: [1, 2, 1] -> [1, 2, 1, 1, 2, 1]
/// 3. Larger varied array: [1, 3, 2, 1] -> [1, 3, 2, 1, 1, 3, 2, 1]
/// </summary>
public static class ConcatenationOfArraySolution
{
    public static int[] GetConcatenation(int[] nums)
    {
        if (nums == null || nums.Length == 0)
            return Array.Empty<int>();

        int n = nums.Length;
        int[] ans = new int[2 * n];

        for (int i = 0; i < n; i++)
        {
            ans[i] = nums[i];
            ans[i + n] = nums[i];
        }

        return ans;
    }

    public static void RunTests()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    DAY 1 LEETCODE: 1929. Concatenation of Array       ");
        Console.WriteLine("=======================================================\n");

        var testCases = new (int[] Input, string CaseName)[]
        {
            (new[] { 1, 2, 1 }, "Test Case 1 (Standard): [1, 2, 1]"),
            (new[] { 1, 3, 2, 1 }, "Test Case 2 (Standard): [1, 3, 2, 1]"),
            (new[] { 7 }, "Test Case 3 (Single Element): [7]"),
            (new[] { -5, 0, 10 }, "Test Case 4 (Negative & Zero): [-5, 0, 10]")
        };

        foreach (var (input, caseName) in testCases)
        {
            int[] result = GetConcatenation(input);
            Console.WriteLine($"{caseName}");
            Console.WriteLine($"  Input:  [{string.Join(", ", input)}]");
            Console.WriteLine($"  Output: [{string.Join(", ", result)}]");
            bool verified = result.Length == input.Length * 2 &&
                            result[0] == input[0] &&
                            result[input.Length] == input[0];
            Console.WriteLine($"  Status: {(verified ? "PASS" : "FAIL")}\n");
        }
    }
}
