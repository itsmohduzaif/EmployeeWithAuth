namespace HRManagement.Models.Settings
{
    public class ThemeSettings
    {
        public int Id { get; set; }
        public string ThemeColor { get; set; }
        public string FontFamily { get; set; }
        public bool IsDarkModeEnabled { get; set; }
    }
}






// we can add these later
//public int Id { get; set; }
//public string PrimaryColor { get; set; } = "#1976d2";
//public string SecondaryColor { get; set; } = "#424242";
//public bool IsDarkMode { get; set; }
//public string FontFamily { get; set; } = "Roboto, sans-serif";
//public string BorderRadius { get; set; } = "4px";
//public string UpdatedBy { get; set; } = string.Empty;
//public DateTime UpdatedAt { get; set; }
