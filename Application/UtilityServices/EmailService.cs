using Infrastructure.Configuration;
using Infrastructure.Core.Interfaces.Application.UtilityServices;
using System.Net;
using System.Net.Mail;

namespace IT_Automation.API.Application.UtilityServices
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        // We use the project-wide singleton config here
        public EmailService()
        {
            _smtpSettings = ProjectConfig.Instance.SmtpSettings; // Accessing the singleton configuration
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = CreateSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, subject, body);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception)
            {
                // Log the exception if needed
                return false;
            }
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = _smtpSettings.Host,
                Port = _smtpSettings.Port,
                EnableSsl = _smtpSettings.EnableSSL,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password)
            };
        }

        private MailMessage CreateMailMessage(string toEmail, string subject, string body)
        {
            return new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }.AddRecipient(toEmail);
        }
    }

}
