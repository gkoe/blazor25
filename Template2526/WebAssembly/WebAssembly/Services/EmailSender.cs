using System.Net;
using System.Net.Mail;
using Base.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace WebAssembly.Services
{
    public class EmailSender(IConfiguration configuration) : IEmailSender<ApplicationUser>
    {
        private readonly IConfiguration _configuration = configuration;

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            string htmlMessage = $"Please confirm your account: {confirmationLink}";
            return SendEmailAsync(email, "Confirm your email", htmlMessage);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            using var client = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["SmtpPort"] ?? "587"),
                Credentials = new NetworkCredential(emailSettings["SmtpUsername"], emailSettings["SmtpPassword"]),
                EnableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SenderEmail"] ?? "admin@htl.at"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            throw new NotImplementedException();
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            throw new NotImplementedException();
        }
    }
}