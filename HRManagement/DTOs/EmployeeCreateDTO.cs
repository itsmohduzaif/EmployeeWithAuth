using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs
{
    public class EmployeeCreateDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; } = null;
        [Required(ErrorMessage = "Role is required.")]
        public string? EmployeeRole { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}