using HRManagement.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.SeedConfiguration
{
    public class EmailSettingsConfiguration : IEntityTypeConfiguration<EmailSettings>
    {
        public void Configure(EntityTypeBuilder<EmailSettings> builder)
        {
            builder.HasData(new EmailSettings
            {
                Id = 1,
                SmtpServer = "smtp.yourhost.com",
                Port = 587,
                UseSSL = true,
                SenderEmail = "noreply@datafirstservices.com",
                SenderName = "HRCorp",
                Username = "smtp-user",
                Password = "smtp-password",
                UpdatedBy = "System",
                UpdatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}
