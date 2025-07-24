using HRManagement.Entities;
using HRManagement.Models;
using HRManagement.Models.Settings;
using HRManagement.SeedConfiguration;
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

    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<GeneralSettings> GeneralSettings { get; set; }
        public DbSet<ThemeSettings> ThemeSettings { get; set; }
        public DbSet<EmailSettings> EmailSettings { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RoleConfiguration());



            builder.Entity<GeneralSettings>().HasData(new GeneralSettings { Id = 1, CompanyName = "HR Corp", SystemLanguage = "en-US", TimeZone = "UTC" });
            builder.Entity<ThemeSettings>().HasData(new ThemeSettings { Id = 1, ThemeColor = "#000000", FontFamily = "Arial", IsDarkModeEnabled = false });
            builder.Entity<EmailSettings>().HasData(new EmailSettings { Id = 1, SmtpServer = "", Port = 587, UseSSL = true, SenderEmail = "", SenderName = "", Username = "", Password = "" });







        }
        public DbSet<Employee> Employees { get; set; }

    }

}
