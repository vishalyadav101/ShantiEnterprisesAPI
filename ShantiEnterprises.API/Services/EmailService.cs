using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> settings,
            ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            bool isHtml = true)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new ArgumentException(
                    "Recipient email is required.");
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException(
                    "Email subject is required.");
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentException(
                    "Email body is required.");
            }

            if (string.IsNullOrWhiteSpace(_settings.SmtpServer))
            {
                throw new InvalidOperationException(
                    "SMTP server is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_settings.SenderEmail))
            {
                throw new InvalidOperationException(
                    "Sender email is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_settings.Username))
            {
                throw new InvalidOperationException(
                    "SMTP username is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_settings.Password))
            {
                throw new InvalidOperationException(
                    "SMTP password is not configured.");
            }

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _settings.SenderName,
                    _settings.SenderEmail));

            message.To.Add(
                MailboxAddress.Parse(toEmail));

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

            try
            {
                using var smtp = new SmtpClient();

                var secureSocketOption =
                    _settings.EnableSsl
                        ? SecureSocketOptions.StartTls
                        : SecureSocketOptions.Auto;

                await smtp.ConnectAsync(
                    _settings.SmtpServer,
                    _settings.Port,
                    secureSocketOption);

                await smtp.AuthenticateAsync(
                    _settings.Username,
                    _settings.Password);

                await smtp.SendAsync(message);

                await smtp.DisconnectAsync(true);

                _logger.LogInformation(
                    "Email sent successfully to {Email}.",
                    toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send email to {Email}.",
                    toEmail);

                throw;
            }
        }
    }
}