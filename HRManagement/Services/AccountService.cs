using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.Entities;
using HRManagement.JwtFeatures;
using HRManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace HRManagement.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtHandler _jwtHandler;
        private readonly SmtpSettings _smtpSettings;
        private readonly AppDbContext _context;

        public AccountService(UserManager<User> userManager, JwtHandler jwtHandler, IOptions<SmtpSettings> smtpSettings, AppDbContext context)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _smtpSettings = smtpSettings.Value;
            _context = context;

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
                UserName = userForRegistration.UserName,
                PhoneNumber = userForRegistration.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, userForRegistration.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return new ApiResponse(false, "User registration failed: " + string.Join(", ", errors), 400, errors);

            }

            //await _userManager.AddToRoleAsync(user, "Employee");
            await _userManager.AddToRoleAsync(user, userForRegistration.EmployeeRole);

            return new ApiResponse(true, "User registered successfully", 200, null);

        }

        public async Task<ApiResponse> Login(UserForAuthenticationDto userForAuthentication)
        {
            var user = await _userManager.FindByEmailAsync(userForAuthentication.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password!))
                return new ApiResponse(false, "Invalid email or password", 401, null);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHandler.CreateToken(user, roles);

            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Email == user.Email);

            return new ApiResponse(true, "Login successful", 200, new { token = token, employee = employee });
        }


        public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new ApiResponse(true, "If the email exists, password reset instructions have been sent.", 200, null);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string resetUrl = $"http://localhost:4040/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

            // Send email via SMTP
            SendForgotPasswordEmail(user.Email, resetUrl);

            return new ApiResponse(true, "If the email exists, password reset instructions have been sent.", 200, null);
        }

        private void SendForgotPasswordEmail(string toEmail, string resetLink)
        {
            var fromAddress = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName);
            var toAddress = new MailAddress(toEmail);
            string subject = "Reset your password";
            string body = $"Hello,\n\nPlease reset your password by clicking the link below:\n{resetLink}\n\nIf you did not request this, please ignore this email.\n\nThanks.";

            var smtp = new SmtpClient
            {
                Host = _smtpSettings.Host,
                Port = _smtpSettings.Port,
                EnableSsl = _smtpSettings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            })
            {
                smtp.Send(message);
            }
        }

        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var decodedToken = Uri.UnescapeDataString(dto.Token);
            

            var user = await _userManager.FindByEmailAsync(dto.Email);
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











        // Token generated from Mailer Send
        //mlsn.afe448e41c372fbc7cbea8586e1190eb629f7ee694b26e9927ce12b163b9e590
        //public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto dto)
        //{
        //    var user = await _userManager.FindByEmailAsync(dto.Email);
        //    if (user == null)
        //        return new ApiResponse(false, "User not found", 404, null);

        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //    // We should email this token to the user in production!
        //    return new ApiResponse(true, "Password reset token generated", 200, new { Email = dto.Email, Token = token });
        //}



        //public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto dto)
        //{
        //    var user = await _userManager.FindByEmailAsync(dto.Email);
        //    if (user == null)
        //        return new ApiResponse(false, "User not found", 404, null);

        //    var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        //    if (!result.Succeeded)
        //    {
        //        var errors = result.Errors.Select(e => e.Description);
        //        return new ApiResponse(false, string.Join(", ", errors), 400, errors);
        //    }

        //    return new ApiResponse(true, "Password reset successfully", 200, null);
        //}


        public async Task<ApiResponse> ChangePasswordAsync(ChangePasswordDto dto, string usernameFromClaim)
        {

            var user = await _userManager.FindByEmailAsync(dto.Email);
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
