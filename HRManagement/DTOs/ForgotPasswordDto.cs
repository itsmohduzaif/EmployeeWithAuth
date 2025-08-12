using System.ComponentModel.DataAnnotations;


namespace HRManagement.DTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
    }

}
