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
using CM.Dtos.Food;

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

        private async Task<bool> ValidateTicketCondition(CreateTicketDto request)
        {
            // Lấy thông tin suất chiếu và phòng
            var showtime = await _dbContext.Showtimes
                .Include(s => s.Room)
                .Include(s => s.Room.Seats)
                .FirstOrDefaultAsync(s => s.Id == request.ShowtimeId);

            if (showtime == null)
            {
                _logger.LogWarning("Showtime not found for ShowtimeId: {ShowtimeId}", request.ShowtimeId);
                throw new Exception("Showtime not found!");
            }

            // Lấy danh sách ghế được chọn
            var seats = await _dbContext.Seats
                .Where(s => request.seatIds.Contains(s.Id) && s.RoomID == showtime.RoomID)
                .OrderBy(s => s.Y)
                .ThenBy(s => s.X)  
                .ToListAsync();

            if (seats.Count != request.seatIds.Count)
            {
                _logger.LogWarning("Invalid seat selection for ShowtimeId: {ShowtimeId}. Some seats are unavailable.", request.ShowtimeId);
                return false;
            }

            // Kiểm tra ghế có liền kề hay không
            for (int i = 0; i < seats.Count - 1; i++)
            {
                // Kiểm tra nếu các ghế có vị trí liền kề (cả theo X và Y)
                if ((seats[i].Y == seats[i + 1].Y && Math.Abs(seats[i].X - seats[i + 1].X) > 1) ||
                    (seats[i].X == seats[i + 1].X && Math.Abs(seats[i].Y - seats[i + 1].Y) > 1))
                {
                    _logger.LogWarning("Seats are not consecutive for ShowtimeId: {ShowtimeId}.", request.ShowtimeId);
                    return false;
                }
            }

            // Lấy toàn bộ ghế trong hàng
            var allSeatsInRow = await _dbContext.Seats
                .Where(s => s.RoomID == showtime.RoomID && s.Y == seats.First().Y) 
                .OrderBy(s => s.X)  
                .ToListAsync();

            // Lấy danh sách ghế đã đặt
            var bookedSeatIds = await _dbContext.Tickets
                .Where(t => t.ShowtimeId == request.ShowtimeId && t.Status != TicketStatus.Canceled)
                .SelectMany(t => t.TicketSeats.Select(ts => ts.SeatId))
                .ToListAsync();

            var combinedSeats = allSeatsInRow
                .Select(s => new
                {
                    s.X,
                    s.Y,
                    s.Id,
                    IsSelected = seats.Any(req => req.Id == s.Id),
                    IsBooked = bookedSeatIds.Contains(s.Id)
                })
                .ToList();

            // Kiểm tra không để lại 1 ghế trống giữa ghế đã chọn và ghế khác
            for (int i = 1; i < combinedSeats.Count - 1; i++)
            {
                if (!combinedSeats[i].IsSelected &&
                    !combinedSeats[i].IsBooked &&
                    combinedSeats[i - 1].IsSelected &&
                    combinedSeats[i + 1].IsSelected)
                {
                    _logger.LogWarning("Isolated seat detected at position: ({X}, {Y}) for ShowtimeId: {ShowtimeId}.", combinedSeats[i].X, combinedSeats[i].Y, request.ShowtimeId);
                    return false; 
                }
            }

            return true;
        }



        public async Task<CMTicket> BookTicketAsync(int userId, CreateTicketDto request, HttpContext context)
        {
            try
            {
                if (!await ValidateTicketCondition(request))
                {
                    throw new Exception("Ticket conditions not met. Cannot book tickets.");
                }   

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
                throw;
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

        public async Task<TicketDto> CreateTicketWithFood(int ticketId, List<FoodItemDto> foodItems)
        {
            try
            {
                var ticket = await _dbContext.Tickets
                    .FirstOrDefaultAsync(t => t.Id == ticketId);

                if (ticket == null)
                {
                    _logger.LogWarning("Ticket not found. TicketId: {TicketId}", ticketId);
                    throw new Exception("Ticket not found.");
                }

                _logger.LogInformation("Ticket found. TicketId: {TicketId}", ticketId);

                // Tính tổng tiền
                decimal foodTotalPrice = CalculateFoodPrice(foodItems);
                _logger.LogInformation("Calculated food total price: {FoodTotalPrice} for TicketId: {TicketId}", foodTotalPrice, ticketId);

                // Cập nhật tổng giá vé
                ticket.TotalPrice += foodTotalPrice;

                // Lưu lại thay đổi
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Ticket updated successfully. TotalPrice: {TotalPrice} for TicketId: {TicketId}", ticket.TotalPrice, ticketId);

                return new TicketDto { Id = ticket.Id, TotalPrice = ticket.TotalPrice };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating ticket with food. TicketId: {TicketId}", ticketId);
                throw;
            }
        }

        private decimal CalculateFoodPrice(List<FoodItemDto> foodItems)
        {
            decimal totalFoodPrice = 0;

            foreach (var item in foodItems)
            {
                totalFoodPrice += item.Quantity * item.Price;
                _logger.LogInformation("FoodItem - FoodId: {FoodId}, Quantity: {Quantity}, Price: {Price}, SubTotal: {SubTotal}",
                    item.FoodId, item.Quantity, item.Price, item.Quantity * item.Price);
            }

            _logger.LogInformation("Total food price calculated: {TotalFoodPrice}", totalFoodPrice);
            return totalFoodPrice;
        }
    }
}
