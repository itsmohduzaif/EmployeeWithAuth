using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.Settings;
using HRManagement.Models.Settings;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Services.Settings
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

            settings.DateFormat = dto.DateFormat;
            settings.Language = dto.Language;
            settings.IsMaintenanceMode = dto.IsMaintenanceMode;

            settings.UpdatedBy = dto.UpdatedBy;
            settings.UpdatedAt = DateTime.UtcNow;

            settings.SupportEmail = dto.SupportEmail;
            settings.DefaultCurrency = dto.DefaultCurrency;

            if (settings.Id == 0)
                await _context.GeneralSettings.AddAsync(settings);
            else
                _context.GeneralSettings.Update(settings);

            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Updated General Settings", 200, settings);
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

            settings.BorderRadius = dto.BorderRadius;

            settings.UpdatedBy = dto.UpdatedBy;
            settings.UpdatedAt = DateTime.UtcNow;

            settings.BackgroundImageUrl = dto.BackgroundImageUrl;
            settings.FontSize = dto.FontSize;

            if (settings.Id == 0)
                await _context.ThemeSettings.AddAsync(settings);
            else
                _context.ThemeSettings.Update(settings);

            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Updated Theme Settings", 200, settings);
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

            settings.UpdatedBy = dto.UpdatedBy;
            settings.UpdatedAt = DateTime.UtcNow;

            if (settings.Id == 0)
                await _context.EmailSettings.AddAsync(settings);
            else
                _context.EmailSettings.Update(settings);

            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Updated Email Settings", 200, settings);
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
                    Body = dto.Body,

                    Description = dto.Description,
                    IsActive = dto.IsActive,

                    UpdatedBy = dto.UpdatedBy,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.EmailTemplates.AddAsync(newTemplate);
            }
            else
            {
                existing.Subject = dto.Subject;
                existing.Body = dto.Body;

                existing.Description = dto.Description;
                existing.IsActive = dto.IsActive;

                existing.UpdatedBy = dto.UpdatedBy;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.EmailTemplates.Update(existing);
            }

            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Email template saved", 200, null);
        }

    }
}
