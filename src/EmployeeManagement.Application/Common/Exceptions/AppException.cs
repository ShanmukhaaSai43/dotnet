namespace EmployeeManagement.Application.Common.Exceptions;

/// <summary>
/// Base exception class for all domain and application-specific exceptions.
/// Maps cleanly to HTTP status codes for RFC 7807 ProblemDetails responses.
/// </summary>
public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public string Title { get; }

    protected AppException(string message, int statusCode, string title = "Application Error")
        : base(message)
    {
        StatusCode = statusCode;
        Title = title;
    }
}
