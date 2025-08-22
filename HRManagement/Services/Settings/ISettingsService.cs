using HRManagement.DTOs;
using HRManagement.DTOs.Settings;

namespace HRManagement.Services.Settings
{
    public interface ISettingsService
    {
        Task<ApiResponse> GetGeneralSettings();
        Task<ApiResponse> UpdateGeneralSettings(GeneralSettingsDto dto);

        Task<ApiResponse> GetThemeSettings();
        Task<ApiResponse> UpdateThemeSettings(ThemeSettingsDto dto);

        Task<ApiResponse> GetEmailSettings();
        Task<ApiResponse> UpdateEmailSettings(EmailSettingsDto dto);

        Task<ApiResponse> GetAllEmailTemplates();
        Task<ApiResponse> AddOrUpdateEmailTemplate(EmailTemplateDto dto);
    }
}
