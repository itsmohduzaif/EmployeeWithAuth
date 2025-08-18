namespace HRManagement.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public string EmployeeRole { get; set; } = string.Empty;
        public string? ProfilePictureFileName { get; set; }  // Actually the BlobName 

    }

}
