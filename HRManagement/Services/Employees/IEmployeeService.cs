using HRManagement.DTOs;
using HRManagement.DTOs.EmployeeDTOs;

namespace HRManagement.Services.Employees
{
    public interface IEmployeeService
    {
        Task<ApiResponse> GetAllEmployees();
        Task<ApiResponse> GetEmployeeById(int id);
        Task<ApiResponse> CreateEmployee(EmployeeCreateDTO employeeDto);
        Task<ApiResponse> SignUpAsAnEmployee(SignUpAsAnEmployeeDTO employeeDto);
        Task<ApiResponse> UpdateEmployee(EmployeeUpdateDTO updated);
        Task<ApiResponse> DeleteEmployee(int id);
        Task<ApiResponse> UpdateProfileAsync(string usernameFromClaim, EmployeeProfileUpdateDTO profileUpdateDto);
        Task<ApiResponse> UploadProfilePictureAsync(string usernameFromClaim, IFormFile file);
        Task<ApiResponse> GetProfileAsync(string username);

    }
}