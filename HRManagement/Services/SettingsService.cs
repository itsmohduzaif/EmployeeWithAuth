using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.Settings;
using HRManagement.Models.Settings;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly AppDbContext _context;

        public SettingsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetGeneralSettings()
        {
            var settings = await _context.GeneralSettings.FirstOrDefaultAsync();
            return new ApiResponse(true, "Fetched", 200, settings);
        }

        public async Task<ApiResponse> UpdateGeneralSettings(GeneralSettingsDto dto)
        {
            var settings = await _context.GeneralSettings.FirstOrDefaultAsync() ?? new GeneralSettings();
            settings.CompanyName = dto.CompanyName;
            settings.SystemLanguage = dto.SystemLanguage;
            settings.TimeZone = dto.TimeZone;

            _context.Update(settings);
            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Updated", 200, settings);
        }

        public async Task<ApiResponse> GetThemeSettings()
        {
            var settings = await _context.ThemeSettings.FirstOrDefaultAsync();
            return new ApiResponse(true, "Fetched", 200, settings);
        }

        public async Task<ApiResponse> UpdateThemeSettings(ThemeSettingsDto dto)
        {
            var settings = await _context.ThemeSettings.FirstOrDefaultAsync() ?? new ThemeSettings();
            settings.ThemeColor = dto.ThemeColor;
            settings.FontFamily = dto.FontFamily;
            settings.IsDarkModeEnabled = dto.IsDarkModeEnabled;

            _context.Update(settings);
            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Updated", 200, settings);
        }

        public async Task<ApiResponse> GetEmailSettings()
        {
            var settings = await _context.EmailSettings.FirstOrDefaultAsync();
            return new ApiResponse(true, "Fetched", 200, settings);
        }

        public async Task<ApiResponse> UpdateEmailSettings(EmailSettingsDto dto)
        {
            var settings = await _context.EmailSettings.FirstOrDefaultAsync() ?? new EmailSettings();

            settings.SmtpServer = dto.SmtpServer;
            settings.Port = dto.Port;
            settings.UseSSL = dto.UseSSL;
            settings.SenderEmail = dto.SenderEmail;
            settings.SenderName = dto.SenderName;
            settings.Username = dto.Username;
            settings.Password = dto.Password;

            _context.Update(settings);
            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Updated", 200, settings);
        }

        public async Task<ApiResponse> GetAllEmailTemplates()
        {
            var templates = await _context.EmailTemplates.ToListAsync();
            return new ApiResponse(true, "Fetched", 200, templates);
        }

        public async Task<ApiResponse> AddOrUpdateEmailTemplate(EmailTemplateDto dto)
        {
            var existing = await _context.EmailTemplates
                .FirstOrDefaultAsync(x => x.TemplateName == dto.TemplateName);

            if (existing == null)
            {
                var newTemplate = new EmailTemplate
                {
                    TemplateName = dto.TemplateName,
                    Subject = dto.Subject,
                    Body = dto.Body
                };
                _context.EmailTemplates.Add(newTemplate);
            }
            else
            {
                existing.Subject = dto.Subject;
                existing.Body = dto.Body;
                _context.EmailTemplates.Update(existing);
            }

            await _context.SaveChangesAsync();
            return new ApiResponse(true, "Email template saved", 200, null);
        }
    }
}
