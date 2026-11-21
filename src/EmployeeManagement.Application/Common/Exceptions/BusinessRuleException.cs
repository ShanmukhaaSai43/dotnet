namespace EmployeeManagement.Application.Common.Exceptions;

public class BusinessRuleException : AppException
{
    public BusinessRuleException(string message) 
        : base(message, 422, "Unprocessable Entity")
    {
    }
}
