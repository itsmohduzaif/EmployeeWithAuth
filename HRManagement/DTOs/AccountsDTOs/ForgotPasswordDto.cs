using System.ComponentModel.DataAnnotations;


namespace HRManagement.DTOs.AccountsDTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string WorkEmail { get; set; }
    }

}
