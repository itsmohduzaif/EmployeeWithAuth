using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs.AccountsDTOs
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string WorkEmail { get; set; }
        [Required(ErrorMessage = "Token is required.")]
        public string Token { get; set; }
        [Required(ErrorMessage = "New Password is required")]
        public string NewPassword { get; set; }
    }

}