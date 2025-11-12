using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs.Leaves.LeaveRequest
{
    public class    CreateLeaveRequestDto
    {
        [Required(ErrorMessage = "Leave type is required.")]
        public int LeaveTypeId { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        public string Reason { get; set; } = string.Empty;

        // New fields for half-day leaves
        public bool IsStartDateHalfDay { get; set; } = false;
        public bool IsEndDateHalfDay { get; set; } = false;



        // Use List<IFormFile> for multiple file uploads
        public List<IFormFile>? Files { get; set; }
    }
}






//using System.ComponentModel.DataAnnotations;


//namespace HRManagement.DTOs.Leaves.LeaveRequest
//{
//    public class CreateLeaveRequestDto
//    {
//        [Required(ErrorMessage = "Leave type is required.")]
//        public int LeaveTypeId { get; set; }
//        [Required(ErrorMessage = "Start date is required.")]
//        public DateTime StartDate { get; set; }
//        [Required(ErrorMessage = "End date is required.")]
//        public DateTime EndDate { get; set; }
//        [Required(ErrorMessage = "Reason is required.")]
//        public string Reason { get; set; } = string.Empty;

//        public IFormFile file { get; set; } = null!; // File is optional, so it can be null
//    }

//}
