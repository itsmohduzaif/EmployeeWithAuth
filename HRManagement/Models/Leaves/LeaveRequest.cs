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

        // Store multiple file names as List<string>
        public List<string>? LeaveRequestFileNames { get; set; }
        //public List<string>? TemporaryBlobUrls { get; set; } // For temporary URLs if needed

        // New field for leave days used (in decimal format)
        public decimal LeaveDaysUsed { get; set; }


    }
}





//using HRManagement.Enums;

//namespace HRManagement.Models.Leaves
//{
//    public class LeaveRequest
//    {
//        public int LeaveRequestId { get; set; }
//        public int EmployeeId { get; set; }
//        public int LeaveTypeId { get; set; }
//        public DateTime StartDate { get; set; }
//        public DateTime EndDate { get; set; }
//        public string Reason { get; set; } = string.Empty;
//        public LeaveRequestStatus Status { get; set; }
//        public string? ManagerRemarks { get; set; }
//        public DateTime RequestedOn { get; set; }
//        public DateTime? ActionedOn { get; set; }
//        public string? LeaveRequestFileName { get; set; } // This is the BlobName, not the full URL

//        //// New property: array of strings
//        //public string[] LeaveRequestFileNames { get; set; } = Array.Empty<string>();
//    }

//}
