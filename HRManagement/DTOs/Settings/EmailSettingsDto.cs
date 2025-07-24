namespace HRManagement.DTOs.Settings
{
    public class EmailSettingsDto
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
