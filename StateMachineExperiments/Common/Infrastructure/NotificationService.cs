using System;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineExperiments.Common.Infrastructure
{
    /// <summary>
    /// Implementation of notification service for LOD case communications.
    /// Integrates with SMTP service for email notifications.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ISmtpService _smtpService;

        public NotificationService(ISmtpService smtpService)
        {
            _smtpService = smtpService ?? throw new ArgumentNullException(nameof(smtpService));
        }

        public async Task NotifyDeterminationAsync(DeterminationNotificationRequest request)
        {
            Console.WriteLine($"[NOTIFICATION] {request.NotificationType} sent to {request.MemberName} ({request.MemberId})");
            Console.WriteLine($"[NOTIFICATION] Case: {request.CaseNumber}");
            Console.WriteLine($"[NOTIFICATION] Determination: {request.Determination}");
            Console.WriteLine($"[NOTIFICATION] Appeal Rights: {request.AppealWindowDays} days from notification date");
            Console.WriteLine($"[NOTIFICATION] Sent at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

            // Send email notification
            var subject = $"Line of Duty Determination - Case {request.CaseNumber}";
            var body = BuildDeterminationEmailBody(request);

            try
            {
                // In production, MemberId would be used to look up actual email address
                await _smtpService.SendEmailAsync($"{request.MemberId}@example.com", subject, body, isHtml: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NOTIFICATION ERROR] Failed to send email: {ex.Message}");
                // In production, you might want to log this error and implement retry logic
            }
        }

        public async Task NotifyAsync(NotificationRequest request)
        {
            Console.WriteLine($"[NOTIFICATION] {request.NotificationType} sent to {request.Recipient}");
            Console.WriteLine($"[NOTIFICATION] Subject: {request.Subject}");
            Console.WriteLine($"[NOTIFICATION] Message: {request.Message}");
            Console.WriteLine($"[NOTIFICATION] Sent at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

            // Send email notification
            try
            {
                await _smtpService.SendEmailAsync(request.Recipient, request.Subject, request.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NOTIFICATION ERROR] Failed to send email: {ex.Message}");
            }
        }

        public async Task AlertStakeholdersAsync(StakeholderAlertRequest request)
        {
            Console.WriteLine($"[ALERT] {request.AlertType} for case {request.CaseNumber}");
            Console.WriteLine($"[ALERT] Message: {request.Message}");
            Console.WriteLine($"[ALERT] Stakeholders notified: {string.Join(", ", request.Stakeholders)}");
            Console.WriteLine($"[ALERT] Sent at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

            // Send email to stakeholders
            var subject = $"Stakeholder Alert - {request.AlertType} - Case {request.CaseNumber}";
            var body = BuildAlertEmailBody(request);

            try
            {
                await _smtpService.SendEmailAsync(request.Stakeholders, subject, body, isHtml: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ALERT ERROR] Failed to send email: {ex.Message}");
            }
        }

        private static string BuildDeterminationEmailBody(DeterminationNotificationRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><body>");
            sb.AppendLine($"<h2>Line of Duty Determination</h2>");
            sb.AppendLine($"<p>Dear {request.MemberName},</p>");
            sb.AppendLine($"<p>This notification is regarding your Line of Duty case <strong>{request.CaseNumber}</strong>.</p>");
            sb.AppendLine($"<p><strong>Determination:</strong> {request.Determination}</p>");
            sb.AppendLine($"<p><strong>Appeal Rights:</strong> You have {request.AppealWindowDays} days from the date of this notification to file an appeal.</p>");
            sb.AppendLine($"<p>If you have any questions, please contact your case administrator.</p>");
            sb.AppendLine($"<p>Sincerely,<br/>Line of Duty System</p>");
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        private static string BuildAlertEmailBody(StakeholderAlertRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><body>");
            sb.AppendLine($"<h2>Stakeholder Alert: {request.AlertType}</h2>");
            sb.AppendLine($"<p><strong>Case Number:</strong> {request.CaseNumber}</p>");
            sb.AppendLine($"<p><strong>Message:</strong></p>");
            sb.AppendLine($"<p>{request.Message}</p>");
            sb.AppendLine($"<p>This is an automated notification from the Line of Duty System.</p>");
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }
    }
}
