using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace StudentHelper.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings?.Value;
            _logger = logger;

            if (_emailSettings == null)
            {
                _logger.LogError("EmailSettings is null - configuration not loaded properly");
            }
            else
            {
                _logger.LogInformation($"EmailService initialized with SMTP: {_emailSettings.SmtpServer}:{_emailSettings.SmtpPort}");
                _logger.LogInformation($"Sender Email: {_emailSettings.SenderEmail}");
            }
        }

        public async Task<bool> SendPasswordResetCodeAsync(string email, string code, string userName)
        {
            string subject = "Код скидування пароля - Student Helper";
            string body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <style>
        body {{ font-family: Arial, sans-serif; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #B3ECFF; padding: 20px; border-radius: 5px; text-align: center; }}
        .content {{ padding: 20px; }}
        .code {{ font-size: 24px; font-weight: bold; color: #0066cc; text-align: center; padding: 20px; background-color: #f0f0f0; border-radius: 5px; }}
        .footer {{ font-size: 12px; color: #999; margin-top: 20px; text-align: center; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Student Helper</h1>
            <h2>Скидування пароля</h2>
        </div>
        <div class='content'>
            <p>Привіт, {userName}!</p>
            <p>Ви запросили скидування пароля. Використайте код нижче:</p>
            <div class='code'>{code}</div>
            <p><strong>Код дійсний протягом 10 хвилин.</strong></p>
            <p>Якщо ви не запитували скидування пароля, ігноруйте цей лист.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 Student Helper. Всі права захищені.</p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                _logger.LogInformation($"Attempting to send email to {to}");
                _logger.LogInformation($"SMTP Server: {_emailSettings.SmtpServer}:{_emailSettings.SmtpPort}");
                _logger.LogInformation($"Sender: {_emailSettings.SenderEmail}");

                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    client.EnableSsl = _emailSettings.EnableSsl;
                    client.Timeout = 10000; // 10 seconds timeout
                    client.Credentials = new NetworkCredential(
                        _emailSettings.SenderEmail,
                        _emailSettings.SenderPassword);

                    using (var mailMessage = new MailMessage(
                        _emailSettings.SenderEmail,
                        to,
                        subject,
                        body))
                    {
                        mailMessage.IsBodyHtml = true;

                        await client.SendMailAsync(mailMessage);
                        _logger.LogInformation($"Email successfully sent to {to}");
                        return true;
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError($"SMTP Error: {smtpEx.Message}. Status Code: {smtpEx.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"SMTP Error: {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email sending failed: {ex.GetType().Name} - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}
