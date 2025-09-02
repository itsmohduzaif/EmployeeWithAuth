using HRManagement.DTOs;
using HRManagement.DTOs.AccountsDTOs;

namespace HRManagement.Services.Accounts
{
    public interface IAccountService
    {
        Task<ApiResponse> Register(UserForRegistrationDto userForRegistration);
        Task<ApiResponse> Login(UserForAuthenticationDto userForAuthentication);
        Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto dto);
        Task<ApiResponse> ChangePasswordAsync(ChangePasswordDto dto, string usernameFromClaim);

    }
}
