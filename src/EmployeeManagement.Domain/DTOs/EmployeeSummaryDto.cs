using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Domain.DTOs;

/// <summary>
/// Projection DTO for LINQ queries.
/// Avoids exposing complete domain entity details.
/// </summary>
public record EmployeeSummaryDto(
    int Id,
    string Name,
    string Email,
    string DepartmentName,
    EmployeeType Type,
    decimal MonthlyPay,
    string City
);

public record DepartmentPayrollReportDto(
    int DepartmentId,
    string DepartmentName,
    int TotalEmployees,
    int FullTimeCount,
    int ContractCount,
    decimal TotalMonthlyPayroll,
    decimal AverageMonthlyPay,
    decimal HighestMonthlyPay,
    decimal MonthlyBudget,
    bool IsOverBudget
);
