using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Enums;
using FluentValidation;

namespace EmployeeManagement.Application.Validators;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
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

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid EmployeeType specified.");

        // Conditional validation for Full-Time vs Contract
        When(x => x.Type == EmployeeType.FullTime, () =>
        {
            RuleFor(x => x.SalaryOrHourlyRate)
                .GreaterThanOrEqualTo(20000m)
                .WithMessage("Full-time annual salary must be at least $20,000.");

            RuleFor(x => x.MonthlyBenefits)
                .GreaterThanOrEqualTo(0m)
                .WithMessage("Monthly benefits allowance cannot be negative.");
        });

        When(x => x.Type == EmployeeType.Contract, () =>
        {
            RuleFor(x => x.SalaryOrHourlyRate)
                .InclusiveBetween(15m, 1000m)
                .WithMessage("Contract hourly rate must be between $15.00 and $1,000.00.");

            RuleFor(x => x.ContractHoursThisMonth)
                .InclusiveBetween(1, 744)
                .WithMessage("Contract hours worked this month must be between 1 and 744 hours.");
        });

        // Address validation (if provided)
        When(x => !string.IsNullOrWhiteSpace(x.ZipCode), () =>
        {
            RuleFor(x => x.ZipCode!)
                .Matches(@"^\d{5}(-\d{4})?$")
                .WithMessage("ZipCode must be a 5-digit number or 5+4 format (e.g. 98101 or 98101-1234).");
        });
    }
}
