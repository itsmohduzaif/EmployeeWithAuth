using HRManagement.Enums;

namespace HRManagement.Models.Leaves
{
    public class LeaveRequest
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveRequestStatus Status { get; set; }
        public string? ManagerRemarks { get; set; }
        public DateTime RequestedOn { get; set; }
        public DateTime? ActionedOn { get; set; }
    }

}
