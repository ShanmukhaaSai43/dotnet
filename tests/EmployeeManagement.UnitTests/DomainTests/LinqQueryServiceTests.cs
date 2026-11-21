using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Services;

namespace EmployeeManagement.UnitTests.DomainTests;

public class LinqQueryServiceTests
{
    private readonly LinqQueryService _service = new();

    [Fact]
    public void GetHighEarners_ShouldFilterCorrectly()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new FullTimeEmployee(1, "High Earner", "h@c.com", 1, annualSalary: 144000m), // $12,000 + $500 = $12,500
            new FullTimeEmployee(2, "Low Earner", "l@c.com", 1, annualSalary: 48000m)     // $4,000 + $500 = $4,500
        };

        // Act
        var highEarners = _service.GetHighEarners(employees, minMonthlyPay: 10000m);

        // Assert
        Assert.Single(highEarners);
        Assert.Equal("High Earner", highEarners[0].Name);
    }

    [Fact]
    public void GetUniqueSkillsAcrossEmployees_ShouldFlattenAndDeduplicate()
    {
        // Arrange
        var skillsData = new List<string[]>
        {
            new[] { "C#", "SQL" },
            new[] { "c#", "Azure", "Docker" }, // Case-insensitive duplicate "c#"
            new[] { "SQL", "Docker" }
        };

        // Act
        var uniqueSkills = _service.GetUniqueSkillsAcrossEmployees(skillsData);

        // Assert
        Assert.Equal(4, uniqueSkills.Count);
        Assert.Contains("Azure", uniqueSkills);
        Assert.Contains("C#", uniqueSkills);
        Assert.Contains("Docker", uniqueSkills);
        Assert.Contains("SQL", uniqueSkills);
    }

    [Fact]
    public void GetPagedEmployees_ShouldReturnCorrectPageSlice()
    {
        // Arrange
        var employees = Enumerable.Range(1, 10)
            .Select(i => (Employee)new FullTimeEmployee(i, $"Emp {i}", $"emp{i}@c.com", 1, 60000m))
            .ToList();

        // Act (Page 2, PageSize 3 => Items 4, 5, 6)
        var (pageItems, totalCount) = _service.GetPagedEmployees(employees, pageNumber: 2, pageSize: 3);

        // Assert
        Assert.Equal(10, totalCount);
        Assert.Equal(3, pageItems.Count);
        Assert.Equal(4, pageItems[0].Id);
        Assert.Equal(5, pageItems[1].Id);
        Assert.Equal(6, pageItems[2].Id);
    }
}
