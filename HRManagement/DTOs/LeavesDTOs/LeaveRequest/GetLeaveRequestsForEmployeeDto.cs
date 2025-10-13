using HRManagement.Enums;

namespace HRManagement.DTOs.Leaves.LeaveRequest
{
    public class GetLeaveRequestsForEmployeeDto
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveRequestStatus Status { get; set; }
        public string? ManagerRemarks { get; set; }
        public DateTime RequestedOn { get; set; }
        public DateTime? ActionedOn { get; set; }

        // Store multiple file names as List<string>
        public List<string>? LeaveRequestFileNames { get; set; }
        public List<string>? TemporaryBlobUrls { get; set; } // For temporary URLs if needed

        // New field for leave days used (in decimal format)
        public decimal LeaveDaysUsed { get; set; }
        public bool IsStartDateHalfDay { get; set; } = false; // New field
        public bool IsEndDateHalfDay { get; set; } = false; // New field




        // New Fields required by Venkatesh
        public string EmployeeName { get; set; } = string.Empty;
        public string LeaveTypeName { get; set; } = string.Empty;



        

    }
}