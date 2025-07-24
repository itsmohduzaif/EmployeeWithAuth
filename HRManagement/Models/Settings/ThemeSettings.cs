namespace HRManagement.Models.Settings
{
    public class ThemeSettings
    {
        public int Id { get; set; }

        public string ThemeColor { get; set; } = "#000000";
        public string FontFamily { get; set; } = "Arial";
        public bool IsDarkModeEnabled { get; set; } = false;

        public string BorderRadius { get; set; } = "4px";

        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Additional useful properties
        public string BackgroundImageUrl { get; set; } = string.Empty;
        public int FontSize { get; set; } = 14; // in px
    }
}
