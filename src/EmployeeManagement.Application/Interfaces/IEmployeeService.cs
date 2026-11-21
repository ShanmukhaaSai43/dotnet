using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeResponse>> GetAllEmployeesAsync(int? departmentId = null, decimal? minPay = null);
    Task<EmployeeResponse> GetEmployeeByIdAsync(int id);
    Task<EmployeeResponse> CreateEmployeeAsync(CreateEmployeeRequest request);
    Task<EmployeeResponse> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request);
    Task<EmployeeResponse> UpdateSalaryAsync(int id, PatchSalaryRequest request);
    Task<bool> DeleteEmployeeAsync(int id);
}
