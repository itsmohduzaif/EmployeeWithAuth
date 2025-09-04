using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.AccountsDTOs;
using HRManagement.Entities;
using HRManagement.JwtFeatures;
using HRManagement.Models;
using HRManagement.Services.Emails;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;



namespace HRManagement.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtHandler _jwtHandler;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public AccountService(UserManager<User> userManager, JwtHandler jwtHandler, AppDbContext context, EmailService emailService)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _context = context;
            _emailService = emailService;

        }

        public async Task<ApiResponse> Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration is null)
                return new ApiResponse(false, "User data is required.", 400, null);

            var user = new User
            {
                EmployeeName = userForRegistration.EmployeeName,
                Email = userForRegistration.WorkEmail,
                UserName = userForRegistration.UserName,
                PhoneNumber = userForRegistration.PersonalPhone
            };
            var result = await _userManager.CreateAsync(user, userForRegistration.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return new ApiResponse(false, "User registration failed: " + string.Join(", ", errors), 400, errors);

            }

            await _userManager.AddToRoleAsync(user, userForRegistration.EmployeeRole);

            return new ApiResponse(true, "User registered successfully", 200, null);

        }

        public async Task<ApiResponse> Login(UserForAuthenticationDto userForAuthentication)
        {
            var user = await _userManager.FindByEmailAsync(userForAuthentication.WorkEmail);
            if (user is null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password!))
                return new ApiResponse(false, "Invalid email or password", 401, null);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHandler.CreateToken(user, roles);

            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.WorkEmail == user.Email);

            return new ApiResponse(true, "Login successful", 200, new { token, employee });
        }


        public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.WorkEmail);
            if (user == null)
                return new ApiResponse(true, "If the email exists, password reset instructions have been sent.", 200, null);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string resetUrl = $"http://localhost:4040/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";



            // Send email via SMTP
            string subject = "Reset your password";
            string body = $"Hello,\n\nPlease reset your password by clicking the link below:\n{resetUrl}\n\nIf you did not request this, please ignore this email.\n\nThanks.";

            _emailService.SendEmail(user.Email, subject, body);


            return new ApiResponse(true, "If the email exists, password reset instructions have been sent.", 200, null);
        }

        
        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var decodedToken = Uri.UnescapeDataString(dto.Token);
            

            var user = await _userManager.FindByEmailAsync(dto.WorkEmail);
            if (user == null)
                return new ApiResponse(false, "User not found", 404, null);

            // using decoded token directly
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
            //var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new ApiResponse(false, string.Join(", ", errors), 400, errors);
            }
            return new ApiResponse(true, "Password reset successfully", 200, null);
        }


        public async Task<ApiResponse> ChangePasswordAsync(ChangePasswordDto dto, string usernameFromClaim)
        {

            var user = await _userManager.FindByEmailAsync(dto.WorkEmail);
            if (user == null)
                return new ApiResponse(false, "User not found", 404, null);

            if (user.UserName != usernameFromClaim) { 
                return new ApiResponse(false, "You can only change your password only.", 400, null);
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new ApiResponse(false, string.Join(", ", errors), 400, errors);
            }

            return new ApiResponse(true, "Password changed successfully", 200, null);
        }
    }
}
