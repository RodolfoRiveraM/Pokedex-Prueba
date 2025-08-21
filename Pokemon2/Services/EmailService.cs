using MailKit.Net.Smtp;
using MimeKit;

namespace Pokemon2.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Envio de correo electrónico con la libreria MailKit
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(
                _configuration["EmailSettings:SmtpServer"],
                int.Parse(_configuration["EmailSettings:Port"]),
                true);
            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
