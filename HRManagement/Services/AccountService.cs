using HRManagement.DTOs;
using HRManagement.Entities;
using HRManagement.JwtFeatures;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRManagement.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtHandler _jwtHandler;

        public AccountService(UserManager<User> userManager, JwtHandler jwtHandler)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;

        }

        public async Task<ApiResponse> Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration is null)
                return new ApiResponse(false, "User data is required.", 400, null);

            var user = new User
            {
                FirstName = userForRegistration.FirstName,
                LastName = userForRegistration.LastName,
                Email = userForRegistration.Email,
                UserName = userForRegistration.Email // Assuming email is used as username
            };
            var result = await _userManager.CreateAsync(user, userForRegistration.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return new ApiResponse(false, "User registration failed: " + string.Join(", ", errors), 400, errors);

            }

            await _userManager.AddToRoleAsync(user, "Employee");

            return new ApiResponse(true, "User registered successfully", 200, null);

        }

        public async Task<ApiResponse> Login(UserForAuthenticationDto userForAuthentication)
        {
            var user = await _userManager.FindByEmailAsync(userForAuthentication.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password!))
                return new ApiResponse(false, "Invalid username or password", 401, null);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHandler.CreateToken(user, roles);
            return new ApiResponse(true, "Login successful", 200, new { token = token });
        }
    }
}
