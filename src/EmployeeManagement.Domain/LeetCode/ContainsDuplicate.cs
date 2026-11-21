namespace EmployeeManagement.Domain.LeetCode;

/// <summary>
/// Day 3 LeetCode Problem: 217. Contains Duplicate
/// Difficulty: Easy
/// 
/// PROBLEM:
/// Given an integer array nums, return true if any value appears at least twice in the array,
/// and return false if every element is distinct.
/// 
/// APPROACHES:
/// 1. Optimal Hash Set Approach (Implemented):
///    - Maintain a HashSet<int>.
///    - Iterate through each number.
///    - Use set.Add(n): If Add returns false, the number already exists -> return true immediately!
///    - If loop finishes without duplicates -> return false.
///    - Time Complexity: O(N) average time.
///    - Space Complexity: O(N) auxiliary space.
/// 
/// 2. Sorting Approach (Alternative):
///    - Sort the array in-place: Array.Sort(nums).
///    - Compare adjacent elements nums[i] == nums[i + 1].
///    - Time Complexity: O(N log N).
///    - Space Complexity: O(1) extra space.
/// 
/// 3. LINQ Approach (One-liner):
///    - nums.Distinct().Count() != nums.Length;
/// </summary>
public static class ContainsDuplicateSolution
{
    /// <summary>
    /// Optimal O(N) Solution using HashSet<int>.
    /// </summary>
    public static bool ContainsDuplicate(int[] nums)
    {
        if (nums == null || nums.Length <= 1)
            return false;

        var seen = new HashSet<int>(nums.Length);
        foreach (int num in nums)
        {
            // HashSet.Add returns false if the item was already present in the set
            if (!seen.Add(num))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// LINQ-based solution demonstrating Modern C# LINQ syntax.
    /// </summary>
    public static bool ContainsDuplicateLinq(int[] nums)
    {
        if (nums == null || nums.Length <= 1)
            return false;

        return nums.Distinct().Count() != nums.Length;
    }

    public static void RunTests()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    DAY 3 LEETCODE: 217. Contains Duplicate            ");
        Console.WriteLine("=======================================================\n");

        var testCases = new (int[] Input, bool Expected, string Description)[]
        {
            (new[] { 1, 2, 3, 1 }, true, "Standard with duplicate 1: [1, 2, 3, 1]"),
            (new[] { 1, 2, 3, 4 }, false, "All unique numbers: [1, 2, 3, 4]"),
            (new[] { 1, 1, 1, 3, 3, 4, 3, 2, 4, 2 }, true, "Multiple duplicates: [1, 1, 1, 3, ...]"),
            (new[] { 42 }, false, "Single element: [42]"),
            (new[] { -1, -1 }, true, "Negative duplicate: [-1, -1]")
        };

        foreach (var (input, expected, description) in testCases)
        {
            bool actualOptimal = ContainsDuplicate(input);
            bool actualLinq = ContainsDuplicateLinq(input);
            bool pass = actualOptimal == expected && actualLinq == expected;

            Console.WriteLine($"{description}");
            Console.WriteLine($"  Input:    [{string.Join(", ", input)}]");
            Console.WriteLine($"  HashSet:  {actualOptimal} | LINQ: {actualLinq} (Expected: {expected})");
            Console.WriteLine($"  Status:   {(pass ? "PASS" : "FAIL")}\n");
        }
    }
}
