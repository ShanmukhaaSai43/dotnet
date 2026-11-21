using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.UnitTests.InfrastructureTests;

public class SqlEmployeeRepositoryTests
{
    private readonly EmployeeManagementDbContext _context;
    private readonly SqlEmployeeRepository _repository;

    public SqlEmployeeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<EmployeeManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new EmployeeManagementDbContext(options);
        _repository = new SqlEmployeeRepository(_context);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var engDept = new Department(1, "Engineering", "ENG", 200000m);
        var hrDept = new Department(2, "Human Resources", "HR", 60000m);

        _context.Departments.AddRange(engDept, hrDept);

        var emp1 = new FullTimeEmployee(
            id: 1,
            name: "Alice Johnson",
            email: "alice.johnson@company.com",
            departmentId: 1,
            annualSalary: 120000m,
            monthlyBenefitsAllowance: 800m,
            address: new Address("100 Tech Way", "Seattle", "WA", "98101")
        );

        var emp2 = new ContractEmployee(
            id: 2,
            name: "Bob Martin",
            email: "bob.martin@contractor.com",
            departmentId: 1,
            hourlyRate: 85m,
            hoursWorkedThisMonth: 160,
            contractEndDate: DateTime.UtcNow.AddMonths(6),
            address: new Address("200 Contract Blvd", "Portland", "OR", "97201")
        );

        var emp3 = new FullTimeEmployee(
            id: 3,
            name: "Carol White",
            email: "carol.white@company.com",
            departmentId: 2,
            annualSalary: 85000m,
            monthlyBenefitsAllowance: 500m
        );

        _context.Employees.AddRange(emp1, emp2, emp3);
        _context.SaveChanges();
    }

    [Fact]
    public void GetAll_ShouldReturnAllEmployeesWithAsNoTracking()
    {
        // Act
        var employees = _repository.GetAll();

        // Assert
        Assert.Equal(3, employees.Count);
        Assert.Contains(employees, e => e.Name == "Alice Johnson");
        Assert.Contains(employees, e => e.Name == "Bob Martin");
        Assert.Contains(employees, e => e.Name == "Carol White");
    }

    [Fact]
    public void GetById_ExistingId_ShouldReturnEmployee()
    {
        // Act
        var employee = _repository.GetById(1);

        // Assert
        Assert.NotNull(employee);
        Assert.Equal("Alice Johnson", employee.Name);
        Assert.Equal(120000m, ((FullTimeEmployee)employee).AnnualSalary);
    }

    [Fact]
    public void GetByEmail_CaseInsensitive_ShouldReturnEmployee()
    {
        // Act
        var employee = _repository.GetByEmail("ALICE.JOHNSON@company.com");

        // Assert
        Assert.NotNull(employee);
        Assert.Equal(1, employee.Id);
    }

    [Fact]
    public void GetByDepartment_ShouldFilterByDepartmentId()
    {
        // Act
        var engEmployees = _repository.GetByDepartment(1);

        // Assert
        Assert.Equal(2, engEmployees.Count);
        Assert.All(engEmployees, e => Assert.Equal(1, e.DepartmentId));
    }

    [Fact]
    public void GetByType_FullTime_ShouldReturnOnlyFullTimeEmployees()
    {
        // Act
        var fullTime = _repository.GetByType<FullTimeEmployee>();

        // Assert
        Assert.Equal(2, fullTime.Count);
        Assert.All(fullTime, e => Assert.Equal(EmployeeType.FullTime, e.Type));
    }

    [Fact]
    public void GetByType_Contract_ShouldReturnOnlyContractEmployees()
    {
        // Act
        var contract = _repository.GetByType<ContractEmployee>();

        // Assert
        Assert.Single(contract);
        Assert.Equal("Bob Martin", contract.First().Name);
        Assert.Equal(EmployeeType.Contract, contract.First().Type);
    }

    [Fact]
    public void Add_ShouldPersistNewEmployee()
    {
        // Arrange
        var newEmployee = new FullTimeEmployee(
            id: 4,
            name: "David New",
            email: "david.new@company.com",
            departmentId: 1,
            annualSalary: 110000m
        );

        // Act
        _repository.Add(newEmployee);

        // Assert
        var retrieved = _repository.GetById(4);
        Assert.NotNull(retrieved);
        Assert.Equal("David New", retrieved.Name);
    }

    [Fact]
    public void Update_ShouldModifyExistingEmployee()
    {
        // Arrange
        var employee = _repository.GetById(1)!;
        employee.UpdateName("Alice Modified");

        // Act
        _repository.Update(employee);

        // Assert
        var updated = _repository.GetById(1);
        Assert.NotNull(updated);
        Assert.Equal("Alice Modified", updated.Name);
    }

    [Fact]
    public void Delete_ExistingEmployee_ShouldReturnTrueAndRemoveEntity()
    {
        // Act
        var success = _repository.Delete(2);

        // Assert
        Assert.True(success);
        Assert.Null(_repository.GetById(2));
    }

    [Fact]
    public async Task GetPagedAsync_FirstPageSize2_ShouldReturnTwoItemsAndTotalCountThree()
    {
        // Arrange
        var pagination = new PaginationParams(PageNumber: 1, PageSize: 2, SortBy: "name", SortDirection: "asc");

        // Act
        var result = await _repository.GetPagedAsync(pagination);

        // Assert
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(2, result.TotalPages);
        Assert.True(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
        Assert.Equal("Alice Johnson", result.Items[0].Name);
        Assert.Equal("Bob Martin", result.Items[1].Name);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_ShouldFilterByNameOrEmail()
    {
        // Arrange
        var pagination = new PaginationParams(SearchTerm: "carol");

        // Act
        var result = await _repository.GetPagedAsync(pagination);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("Carol White", result.Items[0].Name);
    }
}
