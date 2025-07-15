using HRManagement.DTOs;

namespace HRManagement.Services
{
    public interface IEmployeeService
    {
        Task<ApiResponse> GetAllEmployees();
        Task<ApiResponse> GetEmployeeById(int id);
        Task<ApiResponse> CreateEmployee(EmployeeCreateDTO employeeDto);
        Task<ApiResponse> UpdateEmployee(EmployeeUpdateDTO updated);
        Task<ApiResponse> DeleteEmployee(int id);
    }
}