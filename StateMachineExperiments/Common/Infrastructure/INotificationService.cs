namespace StateMachineExperiments.Common.Infrastructure
{
    /// <summary>
    /// Service for sending notifications to members and stakeholders.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Sends a notification to a member with determination results.
        /// </summary>
        /// <param name="caseNumber">The LOD case number</param>
        /// <param name="memberId">The member's ID</param>
        /// <param name="memberName">The member's name</param>
        /// <param name="determination">The LOD determination result</param>
        /// <param name="appealWindowDays">Number of days member has to appeal</param>
        /// <param name="notificationType">Type of notification (e.g., Email, Letter)</param>
        void SendDeterminationNotification(
            string caseNumber,
            string memberId,
            string memberName,
            string determination,
            int appealWindowDays,
            string notificationType = "Email");

        /// <summary>
        /// Sends a general notification.
        /// </summary>
        /// <param name="recipient">The notification recipient</param>
        /// <param name="subject">The notification subject</param>
        /// <param name="message">The notification message</param>
        /// <param name="notificationType">Type of notification (e.g., Email, SMS)</param>
        void SendNotification(
            string recipient,
            string subject,
            string message,
            string notificationType = "Email");

        /// <summary>
        /// Sends an alert to stakeholders (e.g., death case notifications).
        /// </summary>
        /// <param name="caseNumber">The LOD case number</param>
        /// <param name="alertType">Type of alert</param>
        /// <param name="message">Alert message</param>
        /// <param name="stakeholders">List of stakeholder identifiers</param>
        void SendStakeholderAlert(
            string caseNumber,
            string alertType,
            string message,
            params string[] stakeholders);
    }
}
