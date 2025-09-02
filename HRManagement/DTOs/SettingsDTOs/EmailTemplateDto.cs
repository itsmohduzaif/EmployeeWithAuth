namespace HRManagement.DTOs.Settings
{
    public class EmailTemplateDto
    {
        public string TemplateName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public string UpdatedBy { get; set; } = string.Empty;
    }
}
