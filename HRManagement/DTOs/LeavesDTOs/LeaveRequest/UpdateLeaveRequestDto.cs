using System.ComponentModel.DataAnnotations;


namespace HRManagement.DTOs.Leaves.LeaveRequest
{
    public class UpdateLeaveRequestDto
    {
        [Required(ErrorMessage = "Leave Type Id is required.")]
        public int LeaveTypeId { get; set; }
        [Required(ErrorMessage = "Start date is required.")]
        public DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "End date is required.")]
        public DateOnly EndDate { get; set; }
        [Required(ErrorMessage = "Reason is required.")]
        public string Reason { get; set; } = string.Empty;
        public List<IFormFile>? Files { get; set; }
        public bool IsStartDateHalfDay { get; set; } = false; // New field
        public bool IsEndDateHalfDay { get; set; } = false; // New field
    }

}
