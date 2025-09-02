namespace HRManagement.DTOs.Settings
{
    public class EmailSettingsDto
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool UseSSL { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
