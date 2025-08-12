using HRManagement.DTOs;
using HRManagement.Services;
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

        // POST https://localhost:7150/api/Account/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegistrationDto userForRegistration)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, errors));
            }

            var Response = await _accountService.Register(userForRegistration);
            return Ok(Response);
        }

        // POST https://localhost:7150/api/Account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, errors));
            }

            var Response = await _accountService.Login(userForAuthentication);
            return Ok(Response);
        }


        // POST: api/Account/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            Console.WriteLine("Inside forgot password");
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            var response = await _accountService.ForgotPasswordAsync(dto);
            return Ok(response);
        }

        [Authorize]
        // POST: api/Account/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);

            var response = await _accountService.ChangePasswordAsync(dto, usernameFromClaim);
            return Ok(response);
        }

        //
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var Response = await _accountService.ResetPasswordAsync(dto);
            return StatusCode(Response.StatusCode, Response);
        }

    }
}
