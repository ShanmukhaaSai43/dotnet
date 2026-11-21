using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Data;

public class EmployeeManagementDbContext : DbContext
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<FullTimeEmployee> FullTimeEmployees => Set<FullTimeEmployee>();
    public DbSet<ContractEmployee> ContractEmployees => Set<ContractEmployee>();
    public DbSet<Department> Departments => Set<Department>();

    public EmployeeManagementDbContext(DbContextOptions<EmployeeManagementDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Discovers and applies all IEntityTypeConfiguration classes in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmployeeManagementDbContext).Assembly);
    }
}
