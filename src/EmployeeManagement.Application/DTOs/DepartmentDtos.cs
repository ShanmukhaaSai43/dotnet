namespace EmployeeManagement.Application.DTOs;

public record CreateDepartmentRequest(
    string Name,
    string Code,
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
