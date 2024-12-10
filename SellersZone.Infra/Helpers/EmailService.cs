using Microsoft.Extensions.Configuration;
using SellersZone.Core.Interfaces;
using System.Net;
using System.Net.Mail;

namespace SellersZone.Infra.Helpers
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML)
        {
            try
            {
                string mailServer = _config["EmailSettings:MailServer"];
                string fromEmail = _config["EmailSettings:FromEmail"];
                string password = _config["EmailSettings:Password"];
                int port = int.Parse(_config["EmailSettings:MailPort"]);

                var client = new SmtpClient(mailServer, port)
                {
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true,
                };

                MailMessage mailMessage = new MailMessage(fromEmail, toEmail, subject, body)
                {
                    IsBodyHtml = isBodyHTML,
                };

                return client.SendMailAsync(mailMessage);
            }
            catch (FormatException formatEx)
            {
                // This could indicate an issue with email formatting
                Console.WriteLine($"Format Exception: {formatEx.Message}");
                throw;
            }
            catch (SmtpException smtpEx)
            {
                // Log SMTP specific exception
                Console.WriteLine($"SMTP Exception: {smtpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Log general exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

    }
}
