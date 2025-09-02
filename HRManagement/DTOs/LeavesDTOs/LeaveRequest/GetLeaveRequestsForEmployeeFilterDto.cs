using HRManagement.Enums;

namespace HRManagement.DTOs.Leaves.LeaveRequest
{
    public class GetLeaveRequestsForEmployeeFilterDto
    {
        public LeaveRequestStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        //public int? EmployeeId { get; set; }
    }

}
