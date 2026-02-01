using System;

namespace StateMachineExperiments.Common.Infrastructure
{
    /// <summary>
    /// Implementation of notification service for LOD case communications.
    /// In production, this would integrate with actual email/SMS/letter services.
    /// </summary>
    public class NotificationService : INotificationService
    {
        public void SendDeterminationNotification(
            string caseNumber,
            string memberId,
            string memberName,
            string determination,
            int appealWindowDays,
            string notificationType = "Email")
        {
            Console.WriteLine($"[NOTIFICATION] {notificationType} sent to {memberName} ({memberId})");
            Console.WriteLine($"[NOTIFICATION] Case: {caseNumber}");
            Console.WriteLine($"[NOTIFICATION] Determination: {determination}");
            Console.WriteLine($"[NOTIFICATION] Appeal Rights: {appealWindowDays} days from notification date");
            Console.WriteLine($"[NOTIFICATION] Sent at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        }

        public void SendNotification(
            string recipient,
            string subject,
            string message,
            string notificationType = "Email")
        {
            Console.WriteLine($"[NOTIFICATION] {notificationType} sent to {recipient}");
            Console.WriteLine($"[NOTIFICATION] Subject: {subject}");
            Console.WriteLine($"[NOTIFICATION] Message: {message}");
            Console.WriteLine($"[NOTIFICATION] Sent at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        }

        public void SendStakeholderAlert(
            string caseNumber,
            string alertType,
            string message,
            params string[] stakeholders)
        {
            Console.WriteLine($"[ALERT] {alertType} for case {caseNumber}");
            Console.WriteLine($"[ALERT] Message: {message}");
            Console.WriteLine($"[ALERT] Stakeholders notified: {string.Join(", ", stakeholders)}");
            Console.WriteLine($"[ALERT] Sent at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        }
    }
}
