using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CM.ApplicationService.Common;
using CM.ApplicationService.Notification.Abstracts;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CM.ApplicationService.Notification.Implements
{
    public class EmailService : ServiceBase, IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IEmailLogicService _emailLogicService;

        public EmailService(
            IConfiguration config,
            CMDbContext dbContext,
            ILogger<EmailService> logger,
            IEmailLogicService emailLogicService
        )
            : base(logger, dbContext)
        {
            _config = config;
            _emailLogicService = emailLogicService;
        }

        public async Task SendEmailAsync(int ticketId) // Thay đổi từ async void thành async Task
        {
            // Lấy nội dung email từ EmailLogicService
            var emailBody = await _emailLogicService.GenerateEmailBodyAsync(ticketId);

            // Lấy thông tin chi tiết vé
            var ticketDetail = await _dbContext.Tickets
                .Where(t => t.Id == ticketId).FirstOrDefaultAsync();
            if (ticketDetail == null)
                throw new Exception("Ticket not found!");

            // Tạo email
            var message = new MailMessage
            {
                From = new MailAddress(_config["Email:FromEmail"]),
                Subject = "Success Booking Ticket",
                Body = emailBody,
                IsBodyHtml = true,
            };

            message.To.Add(new MailAddress(ticketDetail.User.Email));

            // Gửi email
            var smtpClient = new System.Net.Mail.SmtpClient(_config["Email:Host"])
            {
                Port = 587,
                Credentials = new NetworkCredential(
                    _config["Email:FromEmail"],
                    _config["Email:Password"]
                ),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(message);
        }
    }
}
