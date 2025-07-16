using HRManagement.DTOs;

namespace HRManagement.Services
{
    public interface IAccountService
    {
        Task<ApiResponse> Register(Register model);
        Task<ApiResponse> Login(Login model);
        Task<ApiResponse> AddRole(string role);
        Task<ApiResponse> AssignRole(UserRole model);
    }
}
