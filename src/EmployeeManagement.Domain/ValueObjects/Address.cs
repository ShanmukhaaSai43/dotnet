namespace EmployeeManagement.Domain.ValueObjects;

/// <summary>
/// C# Record demonstrating Value Equality, Immutability, and Non-destructive Mutation.
/// In records, equality is determined by value (contents) rather than memory reference.
/// </summary>
public record Address(string Street, string City, string State, string ZipCode)
{
    public override string ToString() => $"{Street}, {City}, {State} {ZipCode}";
}
