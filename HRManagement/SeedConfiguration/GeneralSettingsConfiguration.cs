using HRManagement.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.SeedConfiguration
{
    public class GeneralSettingsConfiguration : IEntityTypeConfiguration<GeneralSettings>
    {
        public void Configure(EntityTypeBuilder<GeneralSettings> builder)
        {
            builder.HasData(new GeneralSettings
            {
                Id = 1,
                CompanyName = "DataFirst Services",
                SystemLanguage = "en-US",
                TimeZone = "UTC",
                DateFormat = "MM/dd/yyyy",
                Language = "English",
                IsMaintenanceMode = false,
                UpdatedBy = "System",
                UpdatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                SupportEmail = "support@datafirstservices.com",
                DefaultCurrency = "USD"
            });
        }
    }
}
