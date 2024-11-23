using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CM.ApplicationService.Common;
using CM.ApplicationService.Ticket.Abstracts;
using CM.Domain.Seat;
using CM.Domain.Ticket;
using CM.Dtos.Ticket;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CM.Application.Ticket.Services
{
    public class TicketService : ServiceBase, ITicketService
    {
        public TicketService(CMDbContext dbContext, ILogger<ServiceBase> logger)
            : base(logger, dbContext) { }

        public async Task<TicketDto> CreateTicketAsync(CreateTicketDto createTicketDto)
        {
            // Lấy thông tin lịch chiếu
            var showtime = await _dbContext
                .Showtimes.Include(s => s.Room) 
                .FirstOrDefaultAsync(s => s.Id == createTicketDto.ShowtimeId);
            if (showtime == null)
                throw new Exception("Showtime not found");

            var roomId = showtime.Room.Id;

            // Tìm các ghế dựa trên hàng, cột và RoomId
            var seatIds = await _dbContext
                .Seats.Where(s =>
                    createTicketDto.Seats.Any(c => c.Row == s.Row && c.Number == s.Number)
                )
                .Select(s => s.Id) 
                .ToListAsync();
            var seats = await _dbContext
                .Seats.Where(s => seatIds.Contains(s.Id)) 
                .ToListAsync();

            if (seats.Count != createTicketDto.Seats.Count)
                throw new Exception("Some seats are invalid");

            // Kiểm tra trạng thái của các ghế
            var invalidSeats = seats
                .Where(s => s.Status == SeatStatus.Pending || s.Status == SeatStatus.Booked)
                .ToList();
            if (invalidSeats.Any())
            {
                throw new Exception(
                    $"The following seats are not available: {string.Join(", ", invalidSeats.Select(s => $"{s.Row}{s.Number}"))}"
                );
            }

            // Tính tổng giá ghế
            decimal totalPrice = seats.Sum(s => s.Price);

            // Tạo vé mới
            var ticket = new CMTicket
            {
                UserId = createTicketDto.UserId,
                ShowtimeId = createTicketDto.ShowtimeId,
                Status = TicketStatus.Pending,
                CreatedDate = DateTime.Now,
                TotalPrice = totalPrice,
            };

            _dbContext.Tickets.Add(ticket);
            await _dbContext.SaveChangesAsync();

            // Thêm ghế vào vé và cập nhật trạng thái ghế
            foreach (var seat in seats)
            {
                var ticketSeat = new CMTicketSeat
                {
                    TicketId = ticket.Id,
                    SeatId = seat.Id,
                    SeatStatus = SeatStatus.Pending,
                };
                _dbContext.TicketSeats.Add(ticketSeat);

                // Cập nhật trạng thái ghế thành "Pending"
                seat.Status = SeatStatus.Pending;
            }

            await _dbContext.SaveChangesAsync();

            // Return TicketDto
            return new TicketDto
            {
                Id = ticket.Id,
                UserId = ticket.UserId,
                ShowtimeId = ticket.ShowtimeId,
                Status = ticket.Status,
                CreatedDate = ticket.CreatedDate,
                TotalPrice = ticket.TotalPrice,
                Seats = seats
                    .Select(s => new TicketSeatDto
                    {
                        SeatId = s.Id,
                        SeatStatus = SeatStatus.Pending,
                    })
                    .ToList(),
            };
        }

        public async Task<bool> DeleteTicketAsync(int ticketId)
        {
            var ticket = await _dbContext.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return false;

            _dbContext.Tickets.Remove(ticket);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<TicketDto> GetTicketByIdAsync(int ticketId)
        {
            var ticket = await _dbContext
                .Tickets.Include(t => t.User) // Thông tin người mua vé
                .Include(t => t.Showtime) // Thông tin lịch chiếu
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                return null;

            // Lấy thông tin các ghế từ CMTicketSeat
            var ticketSeats = await _dbContext
                .TicketSeats.Where(ts => ts.TicketId == ticketId)
                .Include(ts => ts.Seat) // Lấy thông tin ghế
                .ToListAsync();

            return new TicketDto
            {
                Id = ticket.Id,
                UserId = ticket.UserId,
                ShowtimeId = ticket.ShowtimeId,
                Status = ticket.Status,
                CreatedDate = ticket.CreatedDate,
                TotalPrice = ticket.TotalPrice,
                Seats = ticketSeats
                    .Select(ts => new TicketSeatDto
                    {
                        SeatId = ts.SeatId,
                        SeatStatus = ts.SeatStatus,
                    })
                    .ToList(),
            };
        }

        public async Task<bool> ProcessPaymentAsync(int ticketId)
        {
            var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                return false;

            // Cập nhật trạng thái vé thành "Paid"
            ticket.Status = TicketStatus.Paid;

            // Cập nhật trạng thái ghế thành "Paid"
            var ticketSeats = await _dbContext
                .TicketSeats.Where(ts => ts.TicketId == ticketId)
                .ToListAsync();

            foreach (var ticketSeat in ticketSeats)
            {
                ticketSeat.SeatStatus = SeatStatus.Booked;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTicketStatusAsync(UpdateTicketDto updateTicketDto)
        {
            var ticket = await _dbContext.Tickets.FindAsync(updateTicketDto.TicketId);
            if (ticket == null)
                return false;

            ticket.Status = updateTicketDto.Status;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
