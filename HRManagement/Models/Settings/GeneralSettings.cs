namespace HRManagement.Models.Settings
{
    public class GeneralSettings
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string SystemLanguage { get; set; }
        public string TimeZone { get; set; }
    }
}

//We can add these later
//public int Id { get; set; }
//public string OrganizationName { get; set; } = string.Empty;
//public string LogoUrl { get; set; } = string.Empty;
//public string TimeZone { get; set; } = "UTC";
//public string DateFormat { get; set; } = "MM/dd/yyyy";
//public string Language { get; set; } = "en-US";
//public bool IsMaintenanceMode { get; set; }
//public string UpdatedBy { get; set; } = string.Empty;
//public DateTime UpdatedAt { get; set; }