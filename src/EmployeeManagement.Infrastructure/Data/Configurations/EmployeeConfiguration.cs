using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.Property(e => e.DepartmentId)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        // Table-Per-Hierarchy (TPH) inheritance mapping with discriminator
        builder.HasDiscriminator<EmployeeType>("EmployeeType")
            .HasValue<FullTimeEmployee>(EmployeeType.FullTime)
            .HasValue<ContractEmployee>(EmployeeType.Contract);

        // Owned Value Object: Address
        builder.OwnsOne(e => e.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(200);

            addressBuilder.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(100);

            addressBuilder.Property(a => a.State)
                .HasColumnName("State")
                .HasMaxLength(50);

            addressBuilder.Property(a => a.ZipCode)
                .HasColumnName("ZipCode")
                .HasMaxLength(20);
        });
    }
}

public class FullTimeEmployeeConfiguration : IEntityTypeConfiguration<FullTimeEmployee>
{
    public void Configure(EntityTypeBuilder<FullTimeEmployee> builder)
    {
        builder.Property(e => e.AnnualSalary)
            .HasPrecision(18, 2);

        builder.Property(e => e.MonthlyBenefitsAllowance)
            .HasPrecision(18, 2);

        builder.Property(e => e.HealthPlanTier)
            .HasMaxLength(50);
    }
}

public class ContractEmployeeConfiguration : IEntityTypeConfiguration<ContractEmployee>
{
    public void Configure(EntityTypeBuilder<ContractEmployee> builder)
    {
        builder.Property(e => e.HourlyRate)
            .HasPrecision(18, 2);

        builder.Property(e => e.HoursWorkedThisMonth);

        builder.Property(e => e.ContractEndDate);
    }
}
