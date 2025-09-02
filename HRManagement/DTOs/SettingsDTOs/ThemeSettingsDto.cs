namespace HRManagement.DTOs.Settings
{
    public class ThemeSettingsDto
    {
        public string ThemeColor { get; set; } = "#000000";
        public string FontFamily { get; set; } = "Arial";
        public bool IsDarkModeEnabled { get; set; }

        public string BorderRadius { get; set; } = "4px";

        public string UpdatedBy { get; set; } = string.Empty;

        // Additional useful properties
        public string BackgroundImageUrl { get; set; } = string.Empty;
        public int FontSize { get; set; } = 14;
    }
}
