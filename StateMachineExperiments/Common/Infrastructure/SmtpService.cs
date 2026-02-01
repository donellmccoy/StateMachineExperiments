using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace StateMachineExperiments.Common.Infrastructure
{
    /// <summary>
    /// SMTP email service implementation using MailKit.
    /// </summary>
    public class SmtpService : ISmtpService
    {
        private readonly SmtpSettings _settings;

        public SmtpService(SmtpSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Sends an email message to a single recipient.
        /// </summary>
        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync([to], subject, body, isHtml);
        }

        /// <summary>
        /// Sends an email message to multiple recipients.
        /// </summary>
        public async Task SendEmailAsync(string[] to, string subject, string body, bool isHtml = false)
        {
            if (to == null || to.Length == 0)
            {
                throw new ArgumentException("At least one recipient is required.", nameof(to));
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            
            foreach (var recipient in to)
            {
                message.To.Add(MailboxAddress.Parse(recipient));
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                // Set timeout
                client.Timeout = _settings.TimeoutSeconds * 1000;

                // Connect to SMTP server
                await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl);

                // Authenticate if credentials are provided
                if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
                {
                    await client.AuthenticateAsync(_settings.Username, _settings.Password);
                }

                // Send the message
                await client.SendAsync(message);

                Console.WriteLine($"[SMTP] Email sent to: {string.Join(", ", to)}");
                Console.WriteLine($"[SMTP] Subject: {subject}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SMTP ERROR] Failed to send email: {ex.Message}");
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
