using HRManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Data
{
    //public class AppDbContext: DbContext
    //{
    //    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    //    public DbSet<Employee> Employees { get; set; }
    //}

    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }  // Add DbSet for Employee
    }

}
