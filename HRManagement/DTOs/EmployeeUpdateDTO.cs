namespace HRManagement.DTOs
{
    public class EmployeeUpdateDTO
    {
        public int EmployeeId { get; set; }  // Required to identify the employee
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
    
    }
}
