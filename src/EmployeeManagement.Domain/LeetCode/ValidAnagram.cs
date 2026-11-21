namespace EmployeeManagement.Domain.LeetCode;

/// <summary>
/// Day 5 LeetCode Problem: 242. Valid Anagram
/// Difficulty: Easy
/// 
/// PROBLEM:
/// Given two strings s and t, return true if t is an anagram of s, and false otherwise.
/// An Anagram is a word or phrase formed by rearranging the letters of a different word or phrase,
/// typically using all the original letters exactly once.
/// 
/// APPROACH (Optimal Character Frequency Array):
/// 1. If s.Length != t.Length, they cannot be anagrams -> return false immediately.
/// 2. Allocate a fixed-size frequency array `int[26]` for lowercase English letters.
/// 3. In a single pass:
///      Increment count for s[i] - 'a'.
///      Decrement count for t[i] - 'a'.
/// 4. If all counts are 0, return true; if any count != 0, return false.
/// 
/// COMPLEXITY:
/// - Time Complexity: O(N) where N is string length (single pass).
/// - Space Complexity: O(1) auxiliary space (fixed array of 26 integers, regardless of N).
/// 
/// EDGE CASES TESTED:
/// 1. Standard valid anagram: "anagram", "nagaram" -> true.
/// 2. Different lengths: "a", "ab" -> false.
/// 3. Same length, different characters: "rat", "car" -> false.
/// 4. Single character identical: "a", "a" -> true.
/// 5. Repeated characters mismatch: "aa", "a" -> false.
/// </summary>
public static class ValidAnagramSolution
{
    public static bool IsAnagram(string s, string t)
    {
        if (s == null || t == null)
            return false;

        if (s.Length != t.Length)
            return false;

        // Fixed-size frequency table for ASCII lowercase characters 'a'..'z'
        int[] charCounts = new int[26];

        for (int i = 0; i < s.Length; i++)
        {
            charCounts[s[i] - 'a']++;
            charCounts[t[i] - 'a']--;
        }

        for (int i = 0; i < 26; i++)
        {
            if (charCounts[i] != 0)
            {
                return false;
            }
        }

        return true;
    }

    public static void RunTests()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    DAY 5 LEETCODE: 242. Valid Anagram                 ");
        Console.WriteLine("=======================================================\n");

        var testCases = new (string S, string T, bool Expected, string Description)[]
        {
            ("anagram", "nagaram", true, "Standard valid anagram: anagram / nagaram"),
            ("rat", "car", false, "Different characters: rat / car"),
            ("listen", "silent", true, "Valid anagram: listen / silent"),
            ("a", "ab", false, "Different lengths: a / ab"),
            ("aa", "bb", false, "Same length different counts: aa / bb")
        };

        foreach (var (s, t, expected, description) in testCases)
        {
            bool actual = IsAnagram(s, t);
            bool pass = actual == expected;
            Console.WriteLine($"{description}");
            Console.WriteLine($"  String 1: \"{s}\" | String 2: \"{t}\"");
            Console.WriteLine($"  Output:   {actual} (Expected: {expected})");
            Console.WriteLine($"  Status:   {(pass ? "PASS" : "FAIL")}\n");
        }
    }
}
