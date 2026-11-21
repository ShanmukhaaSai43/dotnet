using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs;

public record CreateDepartmentRequest(
    [Required(ErrorMessage = "Department name is required.")]
    [StringLength(100, MinimumLength = 2)]
    string Name,

    [Required(ErrorMessage = "Department code is required.")]
    [StringLength(10, MinimumLength = 2)]
    string Code,

    [Range(1000, 10000000, ErrorMessage = "Monthly budget must be at least $1,000.")]
    decimal MonthlyBudget
);

public record DepartmentResponse(
    int Id,
    string Name,
    string Code,
    decimal MonthlyBudget,
    string Status,
    int EmployeeCount,
    DateTime CreatedAt
);

public record DepartmentPayrollSummaryResponse(
    int DepartmentId,
    string DepartmentName,
    decimal MonthlyBudget,
    decimal TotalMonthlyPayroll,
    bool IsOverBudget,
    int ActiveHeadcount,
    List<EmployeeResponse> Employees
);
