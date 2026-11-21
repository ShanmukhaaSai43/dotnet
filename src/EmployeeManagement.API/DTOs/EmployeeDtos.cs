using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.API.DTOs;

public record CreateEmployeeRequest(
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    string Name,

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string Email,

    [Range(1, int.MaxValue, ErrorMessage = "A valid DepartmentId must be specified.")]
    int DepartmentId,

    [Required(ErrorMessage = "Employee type (FullTime = 1, Contract = 2) must be specified.")]
    EmployeeType Type,

    [Range(15, 1000000, ErrorMessage = "Compensation must be positive.")]
    decimal SalaryOrHourlyRate,

    decimal MonthlyBenefits = 500m,
    int ContractHoursThisMonth = 160,
    DateTime? ContractEndDate = null,
    string? Street = null,
    string? City = null,
    string? State = null,
    string? ZipCode = null
);

public record UpdateEmployeeRequest(
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2)]
    string Name,

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    string Email,

    [Range(1, int.MaxValue)]
    int DepartmentId,

    string? Street,
    string? City,
    string? State,
    string? ZipCode
);

public record PatchSalaryRequest(
    [Range(15, 1000000, ErrorMessage = "Updated compensation must be at least $15/hr or $20,000/yr.")]
    decimal NewSalaryOrRate
);

public record EmployeeResponse(
    int Id,
    string Name,
    string Email,
    int DepartmentId,
    string Type,
    decimal MonthlyPay,
    string? City,
    bool IsActive,
    DateTime CreatedAt
);
