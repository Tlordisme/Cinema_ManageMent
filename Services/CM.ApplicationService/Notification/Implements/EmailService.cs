using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CM.ApplicationService.Common;
using CM.ApplicationService.Notification.Abstracts;
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
    public class EmailService : ServiceBase, IEmailService
    {
        private readonly IConfiguration _config;
        
        public EmailService(
            IConfiguration config,
            CMDbContext dbContext,
            ILogger<EmailService> logger
        )
            : base(logger, dbContext)
        {
            _config = config;
           
        }

        public async Task SendEmailAsync(int ticketId) // Thay đổi từ async void thành async Task
        {
            // Lấy thông tin chi tiết vé
            var ticketDetail = await _dbContext.Tickets
                .Where(t => t.Id == ticketId).FirstOrDefaultAsync();
            if (ticketDetail == null)
                throw new Exception("Ticket not found!");
            var user = await _dbContext.Users.Where(u => u.Id == ticketDetail.UserId).FirstOrDefaultAsync();
            var showtime = await _dbContext.Showtimes.Where(st => st.Id == ticketDetail.ShowtimeId).FirstOrDefaultAsync();
            var movie = await _dbContext.Movies.Where(m => m.Id == showtime.MovieID).FirstOrDefaultAsync();
            var room = await _dbContext.Rooms.Where(r => r.Id == showtime.RoomID).FirstOrDefaultAsync();
            var theater = await _dbContext.Theaters.Where(r => r.Id == room.TheaterId).FirstOrDefaultAsync();

            var seats = _dbContext.TicketSeats
                .Where(ts => ts.TicketId == ticketId)
                .Select(ts => new SeatForTicketDto
                {
                    Name = ts.Seat.Name,
                    SeatType = ts.Seat.SeatType
                }).ToList();

            // Tạo nội dung email
            var seatsHtml = string.Join(
                "",
                seats.Select(seat =>
                    $"<tr><td>{seat.Name}</td><td>{seat.SeatType}</td></tr>"
                )
            );

            var emailBody =
                $@"
        <html>
            <body>
                <h2>Booking Confirmation</h2>
                <p>Dear {user.FullName},</p>
                <p>Thank you for booking your ticket. Here are the details of your booking:</p>
                <table border='1' style='border-collapse: collapse; width: 100%; text-align: left;'>
                    <tr>
                        <th>Ticket ID</th>
                        <td>{ticketDetail.Id}</td>
                    </tr>
                    <tr>
                        <th>Movie</th>
                        <td>{movie.Title}</td>
                    </tr>
                    <tr>
                        <th>Theater</th>
                        <td>{room.Name}</td>
                    </tr>
                    <tr>
                        <th>Theater</th>
                        <td>{theater.Location}</td>
                    </tr>
                    <tr>
                        <th>Room</th>
                        <td>{room.Name}</td>
                    </tr>
                    <tr>
                        <th>Showtime</th>
                        <td>{showtime.StartTime}</td>
                    </tr>
                    <tr>
                        <th>Booking Date</th>
                        <td>{ticketDetail.BookingDate:yyyy-MM-dd HH:mm}</td>
                    </tr>
                </table>
                <h3>Seats:</h3>
                <table border='1' style='border-collapse: collapse; width: 100%; text-align: left;'>
                    <tr>
                        <th>Seat Name</th>
                        <th>Seat Type</th>
                    </tr>
                    {seatsHtml}
                </table>
                <p>Enjoy your movie!</p>
            </body>
        </html>";

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



