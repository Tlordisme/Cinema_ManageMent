using CM.ApplicationService.Common;
using CM.ApplicationService.Ticket.Abstracts;
using CM.Dtos.Seat;
using CM.Dtos.Ticket;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Ticket.Implements
{
    public class TicketRepository : ServiceBase, ITicketRepository
    {
        public TicketRepository(ILogger<TicketRepository> logger, CMDbContext dbContext) : base(logger, dbContext)
        {
        }

        public async Task<TicketDetailsDto> GetTicketDetailsAsync(int ticketId)
        {
           
                var seats = await _dbContext.TicketSeats
                         .Where(ts => ts.TicketId == ticketId)
                         .Select(ts => ts.Seat)
                         .ToListAsync();
                var ticketDetails = await _dbContext.Tickets
                    .Where(t => t.Id == ticketId)
                    .Select(t => new TicketDetailsDto
                    {
                        TicketId = t.Id,
                        UserName = t.User.FullName,
                        Email = t.User.Email,
                        StartTime = t.Showtime.StartTime,
                        MovieName = t.Showtime.Movie.Title,
                        TheaterName = t.Showtime.Room.Theater.Name,
                        Location = t.Showtime.Room.Theater.Location,
                        RoomName = t.Showtime.Room.Name,
                        BookingDate = t.BookingDate,
                        Seats = _dbContext.TicketSeats
                         .Where(ts => ts.TicketId == ticketId)
                         .Select(ts => new SeatForTicketDto
                         {
                             Name = ts.Seat.Name,
                             SeatType = ts.Seat.SeatType
                         }).ToList(),
                        TotalPrice = t.TotalPrice
                    })
                    .FirstOrDefaultAsync();


                return ticketDetails;
        }
    }
}
