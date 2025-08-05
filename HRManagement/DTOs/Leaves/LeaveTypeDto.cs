using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs.Leaves
{
    public class LeaveTypeDto
    {
        [Required(ErrorMessage = "Leave Type ID is required.")]
        public int LeaveTypeId { get; set; }  // for update
        [Required(ErrorMessage = "Leave Type Name is required.")]
        public string LeaveTypeName { get; set; }
        [Required(ErrorMessage = "Leave Type Description is required.")]
        public string LeaveTypeDescription { get; set; }
        [Range(0, 30, ErrorMessage = "Default Annual Allocation must be a non-negative integer less than 30.")]
        public int DefaultAnnualAllocation { get; set; }
    }
}
