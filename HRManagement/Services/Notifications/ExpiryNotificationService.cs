using HRManagement.Data;
using HRManagement.Models;
using HRManagement.Services.Emails;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Services.Notifications
{
    public class ExpiryNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExpiryNotificationService> _logger;

        public ExpiryNotificationService(IServiceScopeFactory scopeFactory, ILogger<ExpiryNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndNotifyExpiries();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking expiry notifications");
                }

                // Run once every 24 hours
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task CheckAndNotifyExpiries()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var thresholdDate = today.AddDays(30);

            var employees = await context.Employees
                .Where(e =>
                    (e.PassportExpiryDate != null && e.PassportExpiryDate.Value == thresholdDate) ||
                    (e.VisaExpiryDate != null && e.VisaExpiryDate.Value == thresholdDate) ||
                    (e.EmiratesIdExpiryDate != null && e.EmiratesIdExpiryDate.Value == thresholdDate) ||
                    (e.LabourCardExpiryDate != null && e.LabourCardExpiryDate.Value == thresholdDate) ||
                    (e.InsuranceExpiryDate != null && e.InsuranceExpiryDate.Value == thresholdDate)
                )
                .ToListAsync();

            foreach (var emp in employees)
            {
                string subject = "Document Expiry Notification";
                string body = BuildExpiryEmailBody(emp, thresholdDate);

                // Send to Employee
                if (!string.IsNullOrEmpty(emp.WorkEmail))
                {
                    emailService.SendEmail(emp.WorkEmail, subject, body);
                }

                // Send to Admin (assuming Admin email is configured in appsettings.json)
                var adminEmail = scope.ServiceProvider.GetRequiredService<IConfiguration>()["Notifications:AdminEmail"];
                //Console.WriteLine("\n\n\n\n\n"+ adminEmail);
                if (!string.IsNullOrEmpty(adminEmail))
                {
                    emailService.SendEmail(adminEmail, subject, body);
                }
            }
        }

        private string BuildExpiryEmailBody(Employee emp, DateOnly thresholdDate)
        {
            var messages = new List<string>();
            if (emp.PassportExpiryDate == thresholdDate)
                messages.Add($"Passport (Expiry: {emp.PassportExpiryDate:dd-MMM-yyyy})");
            if (emp.VisaExpiryDate == thresholdDate)
                messages.Add($"Visa (Expiry: {emp.VisaExpiryDate:dd-MMM-yyyy})");
            if (emp.EmiratesIdExpiryDate == thresholdDate)
                messages.Add($"Emirates ID (Expiry: {emp.EmiratesIdExpiryDate:dd-MMM-yyyy})");
            if (emp.LabourCardExpiryDate == thresholdDate)
                messages.Add($"Labour Card (Expiry: {emp.LabourCardExpiryDate:dd-MMM-yyyy})");
            if (emp.InsuranceExpiryDate == thresholdDate)
                messages.Add($"Insurance (Expiry: {emp.InsuranceExpiryDate:dd-MMM-yyyy})");

            return
                $"Hi {emp.EmployeeName},\n\n" +
                $"This is a reminder that the following document(s) will expire in 30 days:\n" +
                $"{string.Join("\n", messages)}\n\n" +
                $"Please take the necessary actions to renew them on time.\n\n" +
                "Thanks,\nHR Team";
        }
    }
}
