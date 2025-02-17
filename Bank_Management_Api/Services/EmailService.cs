using Bank_Management_Api.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace Bank_Management_Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"], "Bank Managment App"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
