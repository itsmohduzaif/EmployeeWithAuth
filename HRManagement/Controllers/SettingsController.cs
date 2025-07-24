using HRManagement.DTOs.Settings;
using HRManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // Uncomment in production if needed
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("general")]
        public async Task<IActionResult> GetGeneralSettings() => Ok(await _settingsService.GetGeneralSettings());

        [HttpPut("general")]
        public async Task<IActionResult> UpdateGeneralSettings(GeneralSettingsDto dto) => Ok(await _settingsService.UpdateGeneralSettings(dto));

        [HttpGet("theme")]
        public async Task<IActionResult> GetThemeSettings() => Ok(await _settingsService.GetThemeSettings());

        [HttpPut("theme")]
        public async Task<IActionResult> UpdateThemeSettings(ThemeSettingsDto dto) => Ok(await _settingsService.UpdateThemeSettings(dto));

        [HttpGet("email")]
        public async Task<IActionResult> GetEmailSettings() => Ok(await _settingsService.GetEmailSettings());

        [HttpPut("email")]
        public async Task<IActionResult> UpdateEmailSettings(EmailSettingsDto dto) => Ok(await _settingsService.UpdateEmailSettings(dto));

        [HttpGet("email-template")]
        public async Task<IActionResult> GetEmailTemplates() => Ok(await _settingsService.GetAllEmailTemplates());

        [HttpPost("email-template")]
        public async Task<IActionResult> AddOrUpdateEmailTemplate(EmailTemplateDto dto) => Ok(await _settingsService.AddOrUpdateEmailTemplate(dto));
    }
}
