using CM.ApplicationService.Email.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using MimeKit;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Email.Implements
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com"; // Máy chủ SMTP
        private readonly int _port = 587; // Cổng SMTP
        private readonly string _fromEmail = "your-email@gmail.com"; // Email gửi
        private readonly string _fromPassword = "your-email-password"; // Mật khẩu

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Cinema Management", _fromEmail));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(_smtpServer, _port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_fromEmail, _fromPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
