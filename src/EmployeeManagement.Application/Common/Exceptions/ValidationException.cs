using FluentValidation.Results;

namespace EmployeeManagement.Application.Common.Exceptions;

/// <summary>
/// Represents validation failures encountered during request processing.
/// Contains field-level error mappings for RFC 7807 ValidationProblemDetails.
/// </summary>
public class ValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message = "One or more validation failures have occurred.")
        : base(message, 400, "Validation Failure")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public ValidationException(string propertyName, string errorMessage)
        : this()
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }
}
