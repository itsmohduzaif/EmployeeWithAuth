namespace HRManagement.DTOs.Settings
{
    public class GeneralSettingsDto
    {
        public string CompanyName { get; set; } = string.Empty;

        public string SystemLanguage { get; set; } = "en-US";
        public string TimeZone { get; set; } = "UTC";

        public string DateFormat { get; set; } = "MM/dd/yyyy";
        public string Language { get; set; } = "English";
        public bool IsMaintenanceMode { get; set; }

        public string UpdatedBy { get; set; } = string.Empty;

        // Optional extras
        public string SupportEmail { get; set; } = string.Empty;
        public string DefaultCurrency { get; set; } = "USD";
    }
}
