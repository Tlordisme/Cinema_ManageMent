using System.Linq;
using System.Threading.Tasks;
using CM.ApplicationService.Notification.Abstracts;
using CM.Dtos.Seat;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CM.ApplicationService.Notification.Implements
{
    public class EmailLogicService : IEmailLogicService
    {
        private readonly CMDbContext _dbContext;

        public EmailLogicService(CMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GenerateEmailBodyAsync(int ticketId)
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

            var seatsHtml = string.Join(
                "",
                seats.Select(seat =>
                    $"<tr><td>{seat.Name}</td><td>{seat.SeatType}</td></tr>"
                )
            );

            return $@"
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
                            <th>Location</th>
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
        }
    }
}
