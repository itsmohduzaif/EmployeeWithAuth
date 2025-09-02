using Microsoft.AspNetCore.Identity;

namespace HRManagement.Entities
{
    public class User : IdentityUser
    {
        public string? EmployeeName { get; set; }
        //public string? FirstName { get; set; }
        //public string? LastName { get; set; }
    }
}
