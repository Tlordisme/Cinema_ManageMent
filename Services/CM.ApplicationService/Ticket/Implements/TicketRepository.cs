using CM.ApplicationService.Ticket.Abstracts;
using CM.Dtos.Ticket;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CM.ApplicationService.Ticket.Implements
{
    public class TicketRepository : ITicketRepository
    {
        private readonly CMDbContext _dbContext;

        public TicketRepository(CMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TicketDetailsDto> GetTicketDetailsAsync(int ticketId)
        {
            var ticketDetails = await _dbContext.Tickets
                .Where(t => t.Id == ticketId)
                .Include(t => t.User)
                .Include(t => t.Showtime)
                    .ThenInclude(s => s.Movie)
                .Select(t => new TicketDetailsDto
                {
                    TicketId = t.Id,
                    UserName = t.User.FullName,
                    Email = t.User.Email,
                    MovieName = t.Showtime.Movie.Title,
                    StartTime = t.Showtime.StartTime,
                    TotalPrice = t.TotalPrice
                })
                .FirstOrDefaultAsync();

            return ticketDetails;
        }

        public async Task<bool> DeleteTicket(int ticketId)
        {
            var ticket = await _dbContext.Tickets.FindAsync(ticketId);
            if (ticket == null) return false;

            _dbContext.Tickets.Remove(ticket);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<TicketDetailsDto>> GetTicketsByUserId(int userId)
        {
            return await _dbContext.Tickets
                .Where(t => t.UserId == userId)
                .Select(t => new TicketDetailsDto
                {
                    TicketId = t.Id,
                    UserName = t.User.FullName,
                    MovieName = t.Showtime.Movie.Title,
                    StartTime = t.Showtime.StartTime,
                    TotalPrice = t.TotalPrice
                })
                .ToListAsync();
        }

        public async Task<List<TicketDetailsDto>> GetAllTickets()
        {
            return await _dbContext.Tickets
                .Select(t => new TicketDetailsDto
                {
                    TicketId = t.Id,
                    UserName = t.User.FullName,
                    MovieName = t.Showtime.Movie.Title,
                    StartTime = t.Showtime.StartTime,
                    TotalPrice = t.TotalPrice
                })
                .ToListAsync();
        }
    }
}
