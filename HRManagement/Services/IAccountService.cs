using HRManagement.DTOs;

namespace HRManagement.Services
{
    public interface IAccountService
    {
        Task<ApiResponse> Register(UserForRegistrationDto userForRegistration);
        Task<ApiResponse> Login(UserForAuthenticationDto userForAuthentication);
    }
}
