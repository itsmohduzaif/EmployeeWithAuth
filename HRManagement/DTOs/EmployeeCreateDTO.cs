namespace HRManagement.DTOs
{
    public class EmployeeCreateDTO
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
            
    }
}
