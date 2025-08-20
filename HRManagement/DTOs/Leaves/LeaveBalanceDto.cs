namespace HRManagement.DTOs.Leaves
{
    public class LeaveBalanceDto
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public int DefaultAnnualAllocation { get; set; }
        public int Used { get; set; }
        public int Remaining { get; set; }
    }

}












//namespace HRManagement.DTOs.Leaves
//{
//    public class LeaveBalanceDto
//    {
//        public int LeaveBalanceId { get; set; }
//        public int EmployeeId { get; set; }
//        public int LeaveTypeId { get; set; }
//        public int TotalAllocated { get; set; }
//        public int Used { get; set; }
//    }

//}
