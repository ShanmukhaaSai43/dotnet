using EmployeeManagement.Application.DTOs;
using FluentValidation;

namespace EmployeeManagement.Application.Validators;

public class PatchSalaryRequestValidator : AbstractValidator<PatchSalaryRequest>
{
    public PatchSalaryRequestValidator()
    {
        RuleFor(x => x.NewSalaryOrRate)
            .GreaterThanOrEqualTo(15m)
            .WithMessage("Compensation must be at least $15/hr or a valid annual salary.");
    }
}
