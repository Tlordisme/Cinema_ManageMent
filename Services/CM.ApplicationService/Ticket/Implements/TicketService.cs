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
    public class TicketService : ServiceBase
    {
        public TicketService(CMDbContext dbContext, ILogger<ServiceBase> logger)
            : base(logger, dbContext) { }

        public async Task<bool> CheckSeatsAvailability(List<int> seatIds)
        {
            var seats = await _dbContext.Seats.Where(s => seatIds.Contains(s.Id)).ToListAsync();
            return seats.All(s => s.Status == SeatStatus.Available); // Kiểm tra tất cả ghế đều có trạng thái Available
        }

        // Đặt ghế
        public async Task<CMTicket> BookSeats(int userId, List<int> seatIds, string showtimeId)
        {
            // Kiểm tra xem tất cả ghế có còn trống không
            if (!await CheckSeatsAvailability(seatIds))
            {
                throw new Exception("Một hoặc nhiều ghế không còn trống.");
            }

            // Tính tổng giá vé
            decimal totalPrice = 0;
            var seats = await _dbContext.Seats.Where(s => seatIds.Contains(s.Id)).ToListAsync();
            foreach (var seat in seats)
            {
                // Tính giá vé cho từng ghế dựa trên loại ghế và thời gian
                var seatPrice = await _dbContext.SeatPrices
                    .Where(sp => sp.SeatType == seat.SeatType && sp.RoomID == seat.RoomID)
                    .FirstOrDefaultAsync();
                if (seatPrice != null)
                {
                    totalPrice += seatPrice.Price;
                }
            }

            // Tạo vé
            var ticket = new CMTicket
            {
                UserId = userId,
                ShowtimeId = showtimeId,
                Status = TicketStatus.Pending,  // Vé đang chờ thanh toán
                BookingDate = DateTime.Now,
                TotalPrice = totalPrice
            };

            _dbContext.Tickets.Add(ticket);
            await _dbContext.SaveChangesAsync();

            // Tạo các kết nối giữa Ticket và Seat
            foreach (var seatId in seatIds)
            {
                var ticketSeat = new CMTicketSeat
                {
                    TicketId = ticket.Id,
                    SeatId = seatId,
                    SeatStatus = SeatStatus.Pending,  
                    
                };

                _dbContext.TicketSeats.Add(ticketSeat);
                var seat = await _dbContext.Seats.FindAsync(seatId);
                seat.Status = SeatStatus.Pending; // Đặt trạng thái ghế thành Pending
            }

            await _dbContext.SaveChangesAsync();

            // Trả về vé với thông tin đã đặt
            return ticket;
        }

        // Cập nhật trạng thái vé thành "Paid" và ghế thành "Booked" sau khi thanh toán
        public async Task<bool> CompletePayment(int ticketId)
        {
            var ticket = await _dbContext.Tickets
                              .Where(t => t.Id == ticketId)   // Lọc vé theo Id
                              .FirstOrDefaultAsync();
            if (ticket == null || ticket.Status != TicketStatus.Pending)
            {
                throw new Exception("Vé không tồn tại hoặc đã thanh toán.");
            }
            var ticketSeats = await _dbContext.TicketSeats
                                      .Where(ts => ts.TicketId == ticket.Id)  // Lọc theo TicketId
                                      .Include(ts => ts.Seat)  // Kết hợp thông tin ghế
                                      .ToListAsync();
            // Cập nhật trạng thái vé thành "Paid"
            ticket.Status = TicketStatus.Paid;
            foreach (var ticketSeat in ticketSeats)
            {
                var seat = await _dbContext.Seats.FindAsync(ticketSeat.SeatId);
                seat.Status = SeatStatus.Booked;  // Cập nhật ghế thành "Booked"
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Hủy vé
        //public async Task<bool> CancelBooking(int ticketId)
        //{
        //    var ticket = await _dbContext.Tickets.Include(t => t.TicketSeats).FirstOrDefaultAsync(t => t.Id == ticketId);
        //    if (ticket == null || ticket.Status == TicketStatus.Canceled)
        //    {
        //        throw new Exception("Vé không tồn tại hoặc đã bị hủy.");
        //    }

        //    // Cập nhật trạng thái vé
        //    ticket.Status = TicketStatus.Canceled;
        //    foreach (var ticketSeat in ticket.TicketSeats)
        //    {
        //        var seat = await _dbContext.Seats.FindAsync(ticketSeat.SeatId);
        //        seat.Status = SeatStatus.Available; // Đặt lại ghế thành Available
        //    }

        //    await _dbContext.SaveChangesAsync();
        //    return true;
        //}
    }
}
