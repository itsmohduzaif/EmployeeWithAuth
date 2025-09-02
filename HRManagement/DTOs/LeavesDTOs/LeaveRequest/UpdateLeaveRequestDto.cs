using System.ComponentModel.DataAnnotations;


namespace HRManagement.DTOs.Leaves.LeaveRequest
{
    public class UpdateLeaveRequestDto
    {
        [Required(ErrorMessage = "Leave Type Id is required.")]
        public int LeaveTypeId { get; set; }
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Reason is required.")]
        public string Reason { get; set; } = string.Empty;
        public List<IFormFile>? Files { get; set; }
    }

}
