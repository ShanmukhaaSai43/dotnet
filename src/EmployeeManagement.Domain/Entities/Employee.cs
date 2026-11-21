using EmployeeManagement.Domain.Common;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;

namespace EmployeeManagement.Domain.Entities;

/// <summary>
/// Abstract Base Employee class demonstrating Encapsulation and Polymorphism.
/// Defines contracts and common behaviors that subclasses specialize.
/// </summary>
public abstract class Employee : BaseEntity
{
    // Encapsulated backing field with validation
    private string _name = string.Empty;
    private string _email = string.Empty;

    public string Name
    {
        get => _name;
        protected set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Employee name cannot be null or empty.", nameof(value));
            _name = value.Trim();
        }
    }

    public string Email
    {
        get => _email;
        protected set
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
                throw new ArgumentException("A valid email address containing '@' is required.", nameof(value));
            _email = value.Trim().ToLowerInvariant();
        }
    }

    public int DepartmentId { get; set; }
    public Address? Address { get; set; }
    public abstract EmployeeType Type { get; }
    public bool IsActive { get; protected set; } = true;

    protected Employee(int id, string name, string email, int departmentId, Address? address = null) 
        : base(id)
    {
        Name = name;
        Email = email;
        DepartmentId = departmentId;
        Address = address;
    }

    /// <summary>
    /// Polymorphic Method: Implemented differently by each subclass (Liskov Substitution Principle).
    /// </summary>
    public abstract decimal CalculateMonthlyPay();

    /// <summary>
    /// Virtual Method: Default implementation that subclasses can optionally override.
    /// </summary>
    public virtual string GetRoleSummary() => $"{Name} ({Type}) - Dept #{DepartmentId}";

    public void UpdateName(string newName)
    {
        Name = newName;
        MarkUpdated();
    }

    public void UpdateEmail(string newEmail)
    {
        Email = newEmail;
        MarkUpdated();
    }

    public void UpdateAddress(Address? newAddress)
    {
        Address = newAddress;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}
