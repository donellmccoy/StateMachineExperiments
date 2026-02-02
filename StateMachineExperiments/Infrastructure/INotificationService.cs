using System.Threading.Tasks;

namespace StateMachineExperiments.Infrastructure
{
    /// <summary>
    /// Request for sending a determination notification to a member.
    /// </summary>
    public class DeterminationNotificationRequest
    {
        public required string CaseNumber { get; init; }
        public required string MemberId { get; init; }
        public required string MemberName { get; init; }
        public required string Determination { get; init; }
        public required int AppealWindowDays { get; init; }
        public string NotificationType { get; init; } = "Email";
    }

    /// <summary>
    /// Request for sending a general notification.
    /// </summary>
    public class NotificationRequest
    {
        public required string Recipient { get; init; }
        public required string Subject { get; init; }
        public required string Message { get; init; }
        public string NotificationType { get; init; } = "Email";
    }

    /// <summary>
    /// Request for sending a stakeholder alert.
    /// </summary>
    public class StakeholderAlertRequest
    {
        public required string CaseNumber { get; init; }
        public required string AlertType { get; init; }
        public required string Message { get; init; }
        public required string[] Stakeholders { get; init; }
    }

    /// <summary>
    /// Service for sending notifications to members and stakeholders.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Sends a notification to a member with determination results.
        /// </summary>
        Task NotifyDeterminationAsync(DeterminationNotificationRequest request);

        /// <summary>
        /// Sends a general notification.
        /// </summary>
        Task NotifyAsync(NotificationRequest request);

        /// <summary>
        /// Sends an alert to stakeholders.
        /// </summary>
        Task AlertStakeholdersAsync(StakeholderAlertRequest request);
    }
}
