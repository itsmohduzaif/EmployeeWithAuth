using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs.Leaves.LeaveRequest
{
    public class ApproveLeaveRequestDto
    {
        [Required(ErrorMessage = "Manager remarks are required.")]
        public string ManagerRemarks { get; set; } = string.Empty;
    }
}
