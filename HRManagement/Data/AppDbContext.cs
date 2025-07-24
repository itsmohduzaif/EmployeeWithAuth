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



            //builder.Entity<GeneralSettings>().HasData(new GeneralSettings
            //{
            //    Id = 1,
            //    CompanyName = "HR Corp",
            //    SystemLanguage = "en-US",
            //    TimeZone = "UTC",
            //    DateFormat = "MM/dd/yyyy",
            //    Language = "English",
            //    IsMaintenanceMode = false,
            //    UpdatedBy = "System",
            //    UpdatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
            //    SupportEmail = "support@hrcorp.com",
            //    DefaultCurrency = "USD"
            //});

            //builder.Entity<ThemeSettings>().HasData(new ThemeSettings
            //{
            //    Id = 1,
            //    ThemeColor = "#000000",
            //    FontFamily = "Arial",
            //    IsDarkModeEnabled = false,
            //    BorderRadius = "4px",
            //    UpdatedBy = "System",
            //    UpdatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
            //    BackgroundImageUrl = "",
            //    FontSize = 14
            //});

            //builder.Entity<EmailSettings>().HasData(new EmailSettings
            //{
            //    Id = 1,
            //    SmtpServer = "smtp.yourhost.com",
            //    Port = 587,
            //    UseSSL = true,
            //    SenderEmail = "noreply@hrcorp.com",
            //    SenderName = "HRCorp",
            //    Username = "smtp-user",
            //    Password = "smtp-password",
            //    UpdatedBy = "System",
            //    UpdatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc)
            //});

            //builder.Entity<EmailTemplate>().HasData(new EmailTemplate
            //{
            //    Id = 1,
            //    TemplateName = "WelcomeEmployee",
            //    Subject = "Welcome to HR Corp!",
            //    Body = "Hello {{FirstName}},\nWelcome onboard!",
            //    Description = "Welcome email sent to new employees",
            //    IsActive = true,
            //    UpdatedBy = "System",
            //    UpdatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc)
            //});






        }
        public DbSet<Employee> Employees { get; set; }

    }

}
