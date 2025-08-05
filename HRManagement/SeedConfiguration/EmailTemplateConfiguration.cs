using HRManagement.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.SeedConfiguration
{
    public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
    {
        public void Configure(EntityTypeBuilder<EmailTemplate> builder)
        {
            builder.HasData(new EmailTemplate
            {
                Id = 1,
                TemplateName = "WelcomeEmployee",
                Subject = "Welcome to DataFirst Services!",
                Body = "Hello {{FirstName}},\nWelcome onboard!",
                Description = "Welcome email sent to new employees",
                IsActive = true,
                UpdatedBy = "System",
                UpdatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}
