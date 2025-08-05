namespace HRManagement.Models.Leaves
{
    public class LeaveBalance
    {
        public int LeaveBalanceId { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int TotalAllocated { get; set; }
        public int Used { get; set; }
        public int Remaining => TotalAllocated - Used;
    }
}
