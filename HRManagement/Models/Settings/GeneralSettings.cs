namespace HRManagement.Models.Settings
{
    public class GeneralSettings
    {
        public int Id { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        // Existing + new properties
        public string SystemLanguage { get; set; } = "en-US";
        public string TimeZone { get; set; } = "UTC";

        public string DateFormat { get; set; } = "MM/dd/yyyy";
        public string Language { get; set; } = "English";
        public bool IsMaintenanceMode { get; set; } = false;

        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Optional extras
        public string SupportEmail { get; set; } = string.Empty;
        public string DefaultCurrency { get; set; } = "USD";
    }
}
