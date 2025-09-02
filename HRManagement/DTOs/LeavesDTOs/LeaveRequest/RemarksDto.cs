using System.ComponentModel.DataAnnotations;


namespace HRManagement.DTOs.Leaves.LeaveRequest
{
    public class RemarksDto
    {
        [Required(ErrorMessage = "Manager remarks are required for rejection.")]
        public string ManagerRemarks { get; set; } = string.Empty;
    }
}
