using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CM.ApplicationService.Common;
using CM.ApplicationService.Notification.Abstracts;
using CM.ApplicationService.Ticket.Abstracts;
using CM.Auth.ApplicantService.Auth.Implements;
using CM.Auth.ApplicantService.Permission.Implements;
using CM.Domain.Ticket;
using CM.Dtos.Seat;
using CM.Dtos.Ticket;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CM.ApplicationService.Notification.Implements
{
    public class EmailService : ServiceBase, INotificationService
    {
        private readonly IConfiguration _config;
        private readonly ITicketRepository _ticketRepository;
        private readonly IEmailTemplateService _emailTemplateService;
        public EmailService(
            IConfiguration config,
            CMDbContext dbContext,
            ILogger<EmailService> logger,
            ITicketRepository ticketRepository,
            IEmailTemplateService emailTemplateService
        )
            : base(logger, dbContext)
        {
            _config = config;
            _ticketRepository = ticketRepository;
            _emailTemplateService = emailTemplateService;
           
        }

        public async Task SendNotification(int ticketId) // Thay đổi từ async void thành async Task
        {
            var ticketDetail = await _ticketRepository.GetTicketDetailsAsync(ticketId);
            var emailBody =  await _emailTemplateService.GenerateEmailContent(ticketDetail);


            // Tạo email
            var message = new MailMessage
            {
                From = new MailAddress(_config["Email:FromEmail"]),
                Subject = "Success Booking Ticket",
                Body = emailBody,
                IsBodyHtml = true,
            };

            message.To.Add(new MailAddress(ticketDetail.Email));

            // Gửi email
            var smtpClient = new System.Net.Mail.SmtpClient(_config["Email:Host"])
            {
                Port = int.Parse(_config["Email:Port"]),
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



