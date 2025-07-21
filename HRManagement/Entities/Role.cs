using Microsoft.AspNetCore.Identity;

namespace HRManagement.Entities
{
    public class Role : IdentityRole
    {
        public string? Description { get; set; }
    }
}
