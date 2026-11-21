using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.DTOs;

public record CreateEmployeeRequest(
    string Name,
    string Email,
    int DepartmentId,
    EmployeeType Type,
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
    string Name,
    string Email,
    int DepartmentId,
    string? Street = null,
    string? City = null,
    string? State = null,
    string? ZipCode = null
);

public record PatchSalaryRequest(
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
