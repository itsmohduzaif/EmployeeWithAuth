using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs.AccountsDTOs
{
    public class UserForRegistrationDto
    {
        //public string? FirstName { get; set; }
        //public string? LastName { get; set; }
        public string? EmployeeName { get; set; }
        
        [Required(ErrorMessage = "Email is required.")]
        public string? WorkEmail { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; } = null;
        public string PersonalPhone { get; set; } = string.Empty;
        public string? EmployeeRole { get; set; }
    }
}
