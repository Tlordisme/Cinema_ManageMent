using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CM.ApplicationService.Common;
using CM.ApplicationService.Payment.Abstracts;
using CM.ApplicationService.Payment.Implements;
using CM.ApplicationService.Ticket.Abstracts;
using CM.Domain.Payment;
using CM.Domain.Seat;
using CM.Domain.Showtime;
using CM.Domain.Ticket;
using CM.Dtos.Seat;
using CM.Dtos.Ticket;
using CM.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
//using static Org.BouncyCastle.Math.EC.ECCurve;

namespace CM.Application.Ticket.Services
{
    public class TicketService : ServiceBase, ITicketService
    {
        private IConfiguration _config;
        private readonly IPaymentService _paymentService;
        public TicketService(CMDbContext dbContext, ILogger<ServiceBase> logger, IConfiguration config, IPaymentService paymentService)
            : base(logger, dbContext) {
            _config = config;    
            _paymentService = paymentService;
        }

        public async Task<CMTicket> BookTicketAsync(int userId, CreateTicketDto request, HttpContext context)
        {
            // Kiểm tra lịch 
            var showtime = await _dbContext.Showtimes.Include(s => s.Room)
                                                     .FirstOrDefaultAsync(s => s.Id == request.ShowtimeId);
            if (showtime == null)
                throw new Exception("Showtime not found!");


            // Lấy thông tin ghế
            var seats = await _dbContext.Seats.Where(s => request.seatIds.Contains(s.Id) &&
                                                          s.RoomID == showtime.RoomID &&
                                                          s.Status == SeatStatus.Available).ToListAsync();

            if (seats.Count != request.seatIds.Count)
                throw new Exception("Chứa ghế ko hợp lệ");

            // Kiểm tra loại ghế
            var seatType = seats.Select(s => s.SeatType).Distinct().ToList();
            if (seatType.Count > 1)
                throw new Exception("Không book 2 loại ghế khác nhau trong một vé");

            // Lấy giá vé
            var seatPrice = await _dbContext.SeatPrices
                                            .Where(sp => sp.SeatType == seatType.First() &&
                                                         sp.RoomID == showtime.RoomID &&
                                                         sp.StartDate <= DateTime.Now &&
                                                         sp.EndDate >= DateTime.Now)
                                            .OrderBy(sp => sp.Price) 
                                            .FirstOrDefaultAsync();

            if (seatPrice == null)
                throw new Exception("Không tìm thấy giá vé");

            // Tính tổng giá vé
            decimal totalPrice = seatPrice.Price * seats.Count;

            // Tạo vé
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

            // Gắn ghế vào vé và cập nhật trạng thái ghế
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

            Hangfire.BackgroundJob.Schedule(() => CancelTicketIfNotPaid(ticket.Id), TimeSpan.FromMinutes(1));

            return ticket;
        }

        public async Task<TicketDetailsDto> GetTicketDetailsAsync(int ticketId)
        {
            var seats = _dbContext.TicketSeats
                     .Where(ts => ts.TicketId == ticketId)
                     .Select(ts => ts.Seat) 
                     .ToList();
            var ticketDetails = await _dbContext.Tickets
                .Where(t => t.Id == ticketId && t.Status == TicketStatus.Paid)
                .Select(t => new TicketDetailsDto
                {
                    TicketId = t.Id,
                    UserName = t.User.FullName, 
                    Email =  t.User.Email,
                    Showtime = t.Showtime.StartTime,
                    MovieName = t.Showtime.Movie.Title,
                    TheaterName = t.Showtime.Room.Theater.Name,
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

        public async Task CancelTicketIfNotPaid(int ticketId)
        {
            var ticket = _dbContext.Tickets.FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null || ticket.Status != TicketStatus.Pending)
                return;

            ticket.Status = TicketStatus.Canceled;

            var seats = _dbContext.TicketSeats
                .Where(ts => ts.TicketId == ticketId)
                .Select(ts => ts.Seat)
                .ToList();

            foreach (var seat in seats)
            {
                seat.Status = SeatStatus.Available;
            }

            _dbContext.SaveChanges();
        }

    }
}
