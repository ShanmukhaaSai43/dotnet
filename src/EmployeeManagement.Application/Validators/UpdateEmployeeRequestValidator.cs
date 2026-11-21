using EmployeeManagement.Application.DTOs;
using FluentValidation;

namespace EmployeeManagement.Application.Validators;

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Employee name is required.")
            .Length(2, 100).WithMessage("Employee name must be between 2 and 100 characters.")
            .Matches(@"^[a-zA-Z\s\.\-']+$").WithMessage("Employee name can only contain letters, spaces, dots, and hyphens.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Email must have a valid domain extension (e.g., user@domain.com).")
            .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("A valid positive DepartmentId is required.");

        When(x => !string.IsNullOrWhiteSpace(x.ZipCode), () =>
        {
            RuleFor(x => x.ZipCode!)
                .Matches(@"^\d{5}(-\d{4})?$")
                .WithMessage("ZipCode must be a 5-digit number or 5+4 format (e.g. 98101 or 98101-1234).");
        });
    }
}
