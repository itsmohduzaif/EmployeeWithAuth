namespace HRManagement.DTOs.Leaves.LeaveRequest
{
    public class LeaveRequestDto
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ManagerRemarks { get; set; }
        public DateTime RequestedOn { get; set; }   
        public DateTime? ActionedOn { get; set; }
    }
}
