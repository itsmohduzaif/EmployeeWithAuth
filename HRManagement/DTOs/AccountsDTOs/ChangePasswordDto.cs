using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs.AccountsDTOs
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string WorkEmail { get; set; }
        [Required(ErrorMessage = "Current Password is required")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "New Password is required")]
        public string NewPassword { get; set; }
    }

}