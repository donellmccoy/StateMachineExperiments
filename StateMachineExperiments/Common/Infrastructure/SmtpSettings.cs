namespace StateMachineExperiments.Common.Infrastructure
{
    /// <summary>
    /// Configuration settings for SMTP email service.
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// SMTP server hostname or IP address.
        /// </summary>
        public string Host { get; set; } = "smtp.gmail.com";

        /// <summary>
        /// SMTP server port (typically 587 for TLS, 465 for SSL, 25 for unencrypted).
        /// </summary>
        public int Port { get; set; } = 587;

        /// <summary>
        /// Username for SMTP authentication.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Password for SMTP authentication.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Enable SSL/TLS encryption.
        /// </summary>
        public bool UseSsl { get; set; } = true;

        /// <summary>
        /// Email address to use as the sender.
        /// </summary>
        public string FromEmail { get; set; } = "noreply@example.com";

        /// <summary>
        /// Display name for the sender.
        /// </summary>
        public string FromName { get; set; } = "LOD System";

        /// <summary>
        /// Timeout in seconds for SMTP operations.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}
