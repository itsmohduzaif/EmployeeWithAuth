using HRManagement.DTOs;
using HRManagement.Services.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRManagement.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        // This endpoint is for testing purposes only and should not be used in production.
        // User Register (for testing only without auth)
        // POST https://localhost:7150/api/Account/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegistrationDto userForRegistration)
        {
            var Response = await _accountService.Register(userForRegistration);
            return Ok(Response);
        }

        // POST https://localhost:7150/api/Account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            var Response = await _accountService.Login(userForAuthentication);
            return Ok(Response);
        }


        // POST: api/Account/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var response = await _accountService.ForgotPasswordAsync(dto);
            return Ok(response);
        }

        [Authorize]
        // POST: api/Account/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);

            var response = await _accountService.ChangePasswordAsync(dto, usernameFromClaim);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var Response = await _accountService.ResetPasswordAsync(dto);
            return StatusCode(Response.StatusCode, Response);
        }

    }
}
