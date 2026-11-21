namespace EmployeeManagement.ConsoleApp.Models;

/// <summary>
/// Value Type (Struct): Copied by value, stored on stack or inlined in containing object.
/// Used to demonstrate value type semantics vs reference type semantics.
/// </summary>
public struct EmployeeAddress
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }

    public EmployeeAddress(string street, string city, string state, string postalCode)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
    }

    public override string ToString() => $"{City}, {State} {PostalCode}";
}

/// <summary>
/// Value Type (Enum): Underlying type is int.
/// </summary>
public enum Department
{
    Engineering = 1,
    HumanResources = 2,
    Finance = 3,
    Marketing = 4,
    Operations = 5
}

/// <summary>
/// Reference Type (Class): Stored on the Managed Heap; variable contains reference (pointer) on stack.
/// Contains both value types, nullable value types, and reference types.
/// </summary>
public class Employee
{
    // Value Types
    public int Id { get; set; }
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    public Department Department { get; set; }

    // Nullable Value Types (can hold null or value)
    public decimal? Bonus { get; set; }

    // Reference Types (Non-nullable by default in C# 8+ nullable context)
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Nullable Reference Type (explicitly annotated with ?)
    public string? PhoneNumber { get; set; }

    // Embedded Value Type (Struct)
    public EmployeeAddress Address { get; set; }

    // Reference Type (Array)
    public string[] Skills { get; set; } = Array.Empty<string>();

    public decimal TotalCompensation => Salary + (Bonus ?? 0m);

    public override string ToString()
    {
        string bonusDisplay = Bonus.HasValue ? $"{Bonus.Value:C}" : "N/A";
        string phoneDisplay = PhoneNumber ?? "Not Provided";
        return $"[ID: {Id}] {Name} | Dept: {Department} | Salary: {Salary:C} | Bonus: {bonusDisplay} | Phone: {phoneDisplay} | City: {Address.City} | Active: {IsActive}";
    }
}
