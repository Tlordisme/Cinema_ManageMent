using CM.ApplicationService.Ticket.Abstracts;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CM.Domain.Seat;
using CM.Domain.Ticket;
using CM.Dtos.Ticket;
using CM.Infrastructure;
using Microsoft.AspNetCore.Http;
using CM.ApplicationService.Common;

namespace CM.Application.Ticket.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly CMDbContext _dbContext;
        private readonly ILogger<TicketService> _logger;

        public TicketService(CMDbContext dbContext, ILogger<TicketService> logger, ITicketRepository ticketRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _ticketRepository = ticketRepository;
        }

        public async Task<CMTicket> BookTicketAsync(int userId, CreateTicketDto request, HttpContext context)
        {
            try
            {
                var showtime = await _dbContext.Showtimes
                    .Include(s => s.Room)
                    .FirstOrDefaultAsync(s => s.Id == request.ShowtimeId);

                if (showtime == null)
                {
                    _logger.LogWarning("Showtime not found for ShowtimeId: {ShowtimeId}", request.ShowtimeId);
                    throw new Exception("Showtime not found!");
                }

                var seats = await _dbContext.Seats
                    .Where(s => request.seatIds.Contains(s.Id) && s.RoomID == showtime.RoomID && s.Status == SeatStatus.Available)
                    .ToListAsync();

                if (seats.Count != request.seatIds.Count)
                {
                    _logger.LogWarning("Invalid seat selection for ShowtimeId: {ShowtimeId}. Selected seats are not available.", request.ShowtimeId);
                    throw new Exception("Invalid seat selection");
                }

                var seatType = seats.Select(s => s.SeatType).Distinct().ToList();
                if (seatType.Count > 1)
                {
                    _logger.LogWarning("Different seat types selected for ShowtimeId: {ShowtimeId}. Only one seat type is allowed.", request.ShowtimeId);
                    throw new Exception("Cannot book different seat types in one ticket");
                }

                var seatPrice = await _dbContext.SeatPrices
                    .Where(sp => sp.SeatType == seatType.First() && sp.RoomID == showtime.RoomID)
                    .OrderBy(sp => sp.Price)
                    .FirstOrDefaultAsync();

                if (seatPrice == null)
                {
                    _logger.LogWarning("Seat price not found for ShowtimeId: {ShowtimeId}, RoomID: {RoomID}, SeatType: {SeatType}", request.ShowtimeId, showtime.RoomID, seatType.First());
                    throw new Exception("Seat price not found");
                }

                decimal totalPrice = seatPrice.Price * seats.Count;

                var ticket = new CMTicket
                {
                    UserId = userId,
                    ShowtimeId = request.ShowtimeId,
                    Status = TicketStatus.Pending,
                    BookingDate = DateTime.Now,
                    TotalPrice = totalPrice
                };

                _dbContext.Tickets.Add(ticket);
                await _dbContext.SaveChangesAsync();

                foreach (var seat in seats)
                {
                    seat.Status = SeatStatus.Pending;
                    var ticketSeat = new CMTicketSeat
                    {
                        TicketId = ticket.Id,
                        SeatId = seat.Id,
                        SeatStatus = SeatStatus.Pending
                    };
                    _dbContext.TicketSeats.Add(ticketSeat);
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Ticket successfully booked by UserId: {UserId}, TicketId: {TicketId}, TotalPrice: {TotalPrice}", userId, ticket.Id, totalPrice);
                return ticket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while booking ticket for UserId: {UserId}", userId);
                throw;  // Re-throw the exception after logging
            }
        }

        public async Task<bool> DeleteTicket(int ticketId)
        {
            try
            {
                var result = await _ticketRepository.DeleteTicket(ticketId);
                if (result)
                {
                    _logger.LogInformation("Ticket deleted successfully. TicketId: {TicketId}", ticketId);
                }
                else
                {
                    _logger.LogWarning("Ticket deletion failed. TicketId: {TicketId} not found", ticketId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting ticket. TicketId: {TicketId}", ticketId);
                return false;
            }
        }

        public async Task<List<TicketDetailsDto>> GetAllTickets()
        {
            return await _ticketRepository.GetAllTickets();
        }

        public async Task<List<TicketDetailsDto>> GetTicketsByUserId(int userId)
        {
            return await _ticketRepository.GetTicketsByUserId(userId);
        }

        public async Task<TicketDetailsDto> GetTicketDetailsAsync(int ticketId)
        {
            return await _ticketRepository.GetTicketDetailsAsync(ticketId);
        }
    }
}
