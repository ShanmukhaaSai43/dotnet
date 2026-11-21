using EmployeeManagement.Domain.LeetCode;

namespace EmployeeManagement.UnitTests.LeetCodeTests;

public class ValidAnagramTests
{
    [Theory]
    [InlineData("anagram", "nagaram", true)]
    [InlineData("rat", "car", false)]
    [InlineData("listen", "silent", true)]
    [InlineData("a", "ab", false)]
    [InlineData("ab", "a", false)]
    [InlineData("a", "a", true)]
    [InlineData("racecar", "carrace", true)]
    [InlineData("hello", "billion", false)]
    public void IsAnagram_ShouldCorrectlyIdentifyAnagrams(string s, string t, bool expected)
    {
        // Arrange & Act
        bool actual = ValidAnagramSolution.IsAnagram(s, t);

        // Assert
        Assert.Equal(expected, actual);
    }
}
