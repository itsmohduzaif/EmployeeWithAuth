namespace HRManagement.Models.Leaves
{
    public class LeaveType
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public string LeaveTypeDescription { get; set; } = string.Empty;
        public int DefaultAnnualAllocation { get; set; }
    }
}
