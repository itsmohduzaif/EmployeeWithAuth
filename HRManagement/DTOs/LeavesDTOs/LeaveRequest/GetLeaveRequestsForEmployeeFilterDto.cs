using HRManagement.Enums;

namespace HRManagement.DTOs.Leaves.LeaveRequest
{
    public class GetLeaveRequestsForEmployeeFilterDto
    {
        public LeaveRequestStatus? Status { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        //public int? EmployeeId { get; set; }
    }

}
