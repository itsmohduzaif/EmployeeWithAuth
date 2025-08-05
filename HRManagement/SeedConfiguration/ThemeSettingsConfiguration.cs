using HRManagement.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.SeedConfiguration
{
    public class ThemeSettingsConfiguration : IEntityTypeConfiguration<ThemeSettings>
    {
        public void Configure(EntityTypeBuilder<ThemeSettings> builder)
        {
            builder.HasData(new ThemeSettings
            {
                Id = 1,
                ThemeColor = "#000000",
                FontFamily = "Arial",
                IsDarkModeEnabled = false,
                BorderRadius = "4px",
                UpdatedBy = "System",
                UpdatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                BackgroundImageUrl = "",
                FontSize = 14
            });
        }
    }
}
