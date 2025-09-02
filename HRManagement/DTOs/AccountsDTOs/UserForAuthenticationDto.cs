using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs.AccountsDTOs
{
    public class UserForAuthenticationDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string? WorkEmail { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
