using HRManagement.DTOs;

namespace HRManagement.Services
{
    public interface IAccountService
    {
        Task<ApiResponse> Register(UserForRegistrationDto userForRegistration);
        Task<ApiResponse> Login(UserForAuthenticationDto userForAuthentication);
        //Task<ApiResponse> AddRole(string role);
        //Task<ApiResponse> AssignRole(UserRole model);
    }
}
