using EmployeeManagement.Application.DTOs;
using FluentValidation;

namespace EmployeeManagement.Application.Validators;

public class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Department name is required.")
            .Length(2, 100).WithMessage("Department name must be between 2 and 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Department code is required.")
            .Length(2, 10).WithMessage("Department code must be between 2 and 10 characters.")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Department code must be uppercase letters, numbers, or hyphens (e.g., 'ENG-01').");

        RuleFor(x => x.MonthlyBudget)
            .GreaterThanOrEqualTo(1000m)
            .WithMessage("Monthly budget must be at least $1,000.");
    }
}
