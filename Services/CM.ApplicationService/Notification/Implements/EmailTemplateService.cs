using CM.ApplicationService.Common;
using CM.ApplicationService.Notification.Abstracts;
using CM.Dtos.Ticket;
using CM.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Notification.Implements
{
    public class EmailTemplateService : ServiceBase, IEmailTemplateService
    {
        public EmailTemplateService(
            CMDbContext dbContext,
            ILogger<EmailTemplateService> logger
        )
            : base(logger, dbContext)
        {
           

        }
        public async Task<string> GenerateEmailContent(TicketDetailsDto ticketDetailsDto)
        {
            var ticketDetail = ticketDetailsDto;
            var seatsHtml = string.Join(
                "",
                ticketDetail.Seats.Select(seat =>
                    $"<tr><td>{seat.Name}</td><td>{seat.SeatType}</td></tr>"
                )
            );

            return $@"
                <html>
                    <body>
                        <h2>Booking Confirmation</h2>
                        <p>Dear {ticketDetail.UserName},</p>
                        <p>Thank you for booking your ticket. Here are the details of your booking:</p>
                        <table border='1' style='border-collapse: collapse; width: 100%; text-align: left;'>
                            <tr><th>Ticket ID</th><td>{ticketDetail.TicketId}</td></tr>
                            <tr><th>Movie</th><td>{ticketDetail.MovieName}</td></tr>
                            <tr><th>Room</th><td>{ticketDetail.RoomName}</td></tr>
                            <tr><th>Theater</th><td>{ticketDetail.TheaterName}</td></tr>
                             <tr><th>Theater</th><td>{ticketDetail.Location}</td></tr>
                            <tr><th>Showtime</th><td>{ticketDetail.StartTime}</td></tr>
                            <tr><th>Booking Date</th><td>{ticketDetail.BookingDate:yyyy-MM-dd HH:mm}</td></tr>
                        </table>
                        <h3>Seats:</h3>
                        <table border='1' style='border-collapse: collapse; width: 100%; text-align: left;'>
                            <tr><th>Seat Name</th><th>Seat Type</th></tr>
                            {seatsHtml}
                        </table>
                        <p>Enjoy your movie!</p>
                    </body>
                </html>";
        }
    }
}
