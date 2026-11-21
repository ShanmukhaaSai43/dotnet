namespace EmployeeManagement.ConsoleApp.Exceptions;

/// <summary>
/// Custom exception demonstrating domain-specific exception handling in C#.
/// </summary>
public class EmployeeValidationException : Exception
{
    public string? FieldName { get; }

    public EmployeeValidationException(string message) : base(message)
    {
    }

    public EmployeeValidationException(string message, string fieldName) : base(message)
    {
        FieldName = fieldName;
    }

    public EmployeeValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class EmployeeNotFoundException : Exception
{
    public int EmployeeId { get; }

    public EmployeeNotFoundException(int id) : base($"Employee with ID '{id}' was not found.")
    {
        EmployeeId = id;
    }
}
