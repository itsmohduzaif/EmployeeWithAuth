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

                //await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task CheckAndNotifyExpiries()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var adminEmail = scope.ServiceProvider.GetRequiredService<IConfiguration>()["Notifications:AdminEmail"];

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

            if (!employees.Any())
                return; // nothing to notify

            
            var messageStringForAdmin = string.Empty;
            foreach (var emp in employees)
            {
                string employeeSubject = "Document Expiry Notification";
                string employeeBody = BuildExpiryEmailBodyForEmployee(emp, thresholdDate);

                // Send to Employee
                if (!string.IsNullOrEmpty(emp.WorkEmail))
                {
                    emailService.SendEmail(emp.WorkEmail, employeeSubject, employeeBody);
                }




                // Prepare message for Admin
                if (!string.IsNullOrEmpty(adminEmail))
                {
                    var messagesForAdmin = new List<string>();
                    if (emp.PassportExpiryDate == thresholdDate)
                        messagesForAdmin.Add($"Passport (Expiry: {emp.PassportExpiryDate:dd-MMM-yyyy})");
                    if (emp.VisaExpiryDate == thresholdDate)
                        messagesForAdmin.Add($"Visa (Expiry: {emp.VisaExpiryDate:dd-MMM-yyyy})");
                    if (emp.EmiratesIdExpiryDate == thresholdDate)
                        messagesForAdmin.Add($"Emirates ID (Expiry: {emp.EmiratesIdExpiryDate:dd-MMM-yyyy})");
                    if (emp.LabourCardExpiryDate == thresholdDate)
                        messagesForAdmin.Add($"Labour Card (Expiry: {emp.LabourCardExpiryDate:dd-MMM-yyyy})");
                    if (emp.InsuranceExpiryDate == thresholdDate)
                        messagesForAdmin.Add($"Insurance (Expiry: {emp.InsuranceExpiryDate:dd-MMM-yyyy})");


                    if (messagesForAdmin.Any())
                    {
                        messageStringForAdmin += $"Employee Name: {emp.EmployeeName} (Employee ID: {emp.EmployeeId})\n" +
                                                    $"{string.Join("\n", messagesForAdmin)}\n\n";
                    }
                    

                }
            }

            // Send to Admin (assuming Admin email is configured in appsettings.json)
            string adminSubject = "Document Expiry Notification";
            string adminBody = "Dear Admin,\n\n" +
                          "The following employees have documents expiring in 30 days:\n\n" +
                          messageStringForAdmin +
                          "Please ensure they are notified accordingly.\n\n" +
                          "Thanks,\nHR Team";

            emailService.SendEmail(adminEmail, adminSubject, adminBody);
        }


        private string BuildExpiryEmailBodyForEmployee(Employee emp, DateOnly thresholdDate)
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



// abstract BackgroundService class 
//public abstract class BackgroundService : IHostedService, IDisposable
//{
//    private Task _executingTask;
//    private CancellationTokenSource _stoppingCts;

//    public Task StartAsync(CancellationToken cancellationToken)
//    {
//        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

//        // 👇 This is the key line: your ExecuteAsync method is called here
//        _executingTask = ExecuteAsync(_stoppingCts.Token);

//        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
//    }

//    public async Task StopAsync(CancellationToken cancellationToken)
//    {
//        if (_executingTask == null)
//        {
//            return;
//        }

//        _stoppingCts.Cancel();

//        await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
//    }

//    public abstract Task ExecuteAsync(CancellationToken stoppingToken);
//}




// Previous version - kept for reference
//using HRManagement.Data;
//using HRManagement.Models;
//using HRManagement.Services.Emails;
//using Microsoft.EntityFrameworkCore;

//namespace HRManagement.Services.Notifications
//{
//    public class ExpiryNotificationService : BackgroundService
//    {
//        private readonly IServiceScopeFactory _scopeFactory;
//        private readonly ILogger<ExpiryNotificationService> _logger;

//        public ExpiryNotificationService(IServiceScopeFactory scopeFactory, ILogger<ExpiryNotificationService> logger)
//        {
//            _scopeFactory = scopeFactory;
//            _logger = logger;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                try
//                {
//                    await CheckAndNotifyExpiries();
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error while checking expiry notifications");
//                }

//                // Run once every 24 hours
//                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
//            }
//        }

//        private async Task CheckAndNotifyExpiries()
//        {
//            using var scope = _scopeFactory.CreateScope();
//            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

//            var today = DateOnly.FromDateTime(DateTime.UtcNow);
//            var thresholdDate = today.AddDays(30);

//            var employees = await context.Employees
//                .Where(e =>
//                    (e.PassportExpiryDate != null && e.PassportExpiryDate.Value == thresholdDate) ||
//                    (e.VisaExpiryDate != null && e.VisaExpiryDate.Value == thresholdDate) ||
//                    (e.EmiratesIdExpiryDate != null && e.EmiratesIdExpiryDate.Value == thresholdDate) ||
//                    (e.LabourCardExpiryDate != null && e.LabourCardExpiryDate.Value == thresholdDate) ||
//                    (e.InsuranceExpiryDate != null && e.InsuranceExpiryDate.Value == thresholdDate)
//                )
//                .ToListAsync();

//            foreach (var emp in employees)
//            {
//                string subject = "Document Expiry Notification";
//                string body = BuildExpiryEmailBody(emp, thresholdDate);

//                // Send to Employee
//                if (!string.IsNullOrEmpty(emp.WorkEmail))
//                {
//                    emailService.SendEmail(emp.WorkEmail, subject, body);
//                }

//                // Send to Admin (assuming Admin email is configured in appsettings.json)
//                var adminEmail = scope.ServiceProvider.GetRequiredService<IConfiguration>()["Notifications:AdminEmail"];
//                //Console.WriteLine("\n\n\n\n\n"+ adminEmail);
//                if (!string.IsNullOrEmpty(adminEmail))
//                {
//                    emailService.SendEmail(adminEmail, subject, body);
//                }
//            }
//        }

//        private string BuildExpiryEmailBody(Employee emp, DateOnly thresholdDate)
//        {
//            var messages = new List<string>();
//            if (emp.PassportExpiryDate == thresholdDate)
//                messages.Add($"Passport (Expiry: {emp.PassportExpiryDate:dd-MMM-yyyy})");
//            if (emp.VisaExpiryDate == thresholdDate)
//                messages.Add($"Visa (Expiry: {emp.VisaExpiryDate:dd-MMM-yyyy})");
//            if (emp.EmiratesIdExpiryDate == thresholdDate)
//                messages.Add($"Emirates ID (Expiry: {emp.EmiratesIdExpiryDate:dd-MMM-yyyy})");
//            if (emp.LabourCardExpiryDate == thresholdDate)
//                messages.Add($"Labour Card (Expiry: {emp.LabourCardExpiryDate:dd-MMM-yyyy})");
//            if (emp.InsuranceExpiryDate == thresholdDate)
//                messages.Add($"Insurance (Expiry: {emp.InsuranceExpiryDate:dd-MMM-yyyy})");

//            return
//                $"Hi {emp.EmployeeName},\n\n" +
//                $"This is a reminder that the following document(s) will expire in 30 days:\n" +
//                $"{string.Join("\n", messages)}\n\n" +
//                $"Please take the necessary actions to renew them on time.\n\n" +
//                "Thanks,\nHR Team";
//        }
//    }
//}
