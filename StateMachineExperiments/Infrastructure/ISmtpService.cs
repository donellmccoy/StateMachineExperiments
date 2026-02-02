using System.Threading.Tasks;

namespace StateMachineExperiments.Infrastructure
{
    /// <summary>
    /// Service for sending emails via SMTP.
    /// </summary>
    public interface ISmtpService
    {
        /// <summary>
        /// Sends an email message.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content (supports HTML).</param>
        /// <param name="isHtml">Indicates whether the body contains HTML content.</param>
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);

        /// <summary>
        /// Sends an email message to multiple recipients.
        /// </summary>
        /// <param name="to">Array of recipient email addresses.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content (supports HTML).</param>
        /// <param name="isHtml">Indicates whether the body contains HTML content.</param>
        Task SendEmailAsync(string[] to, string subject, string body, bool isHtml = false);
    }
}
