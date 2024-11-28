using CM.ApplicationService.Email.Abstracts;
using CM.ApplicationService.Ticket.Abstracts;
using CM.Domain.Ticket;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Ticket.Implements
{
    public class TicketService : ITicketService
    {
        private readonly CMDbContext _context;
        private readonly IEmailService _emailService;

        public TicketService(CMDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<CMTicket> CreateTicket(int userId, string showtimeId, List<int> seatIds)
        {
            var showtime = await _context.Showtimes.FirstOrDefaultAsync(s => s.Id == showtimeId);
            if (showtime == null) throw new Exception("Showtime not found");

            var seats = await _context.Seats.Where(s => seatIds.Contains(s.Id)).ToListAsync();
            if (seats.Count != seatIds.Count) throw new Exception("Some seats not found");

            var ticket = new CMTicket
            {
                UserId = userId,
                ShowtimeId = showtimeId,
                Status = TicketStatus.Pending,
                CreatedDate = DateTime.Now,
                TotalPrice = seats.Sum(s => s.Price),
                TicketSeats = seats.Select(s => new CMTicketSeat
                {
                    SeatId = s.Id,
                    SeatStatus = SeatStatus.Pending
                }).ToList()
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> PayTicket(int ticketId)
        {
            var ticket = await _context.Tickets.Include(t => t.TicketSeats)
                                                .ThenInclude(ts => ts.Seat)
                                                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null || ticket.Status != TicketStatus.Pending)
                return false;

            ticket.Status = TicketStatus.Paid;
            foreach (var seat in ticket.TicketSeats)
            {
                seat.SeatStatus = SeatStatus.Paid;
            }

            await _context.SaveChangesAsync();

            // Gửi email xác nhận
            var emailBody = BuildTicketConfirmationEmail(ticket);
            var userEmail = (await _context.Users.FindAsync(ticket.UserId))?.Email;

            if (!string.IsNullOrEmpty(userEmail))
            {
                await _emailService.SendEmailAsync(userEmail, "Xác nhận vé", emailBody);
            }

            return true;
        }

        public async Task<bool> CancelTicket(int ticketId)
        {
            var ticket = await _context.Tickets.Include(t => t.TicketSeats)
                                                .FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null || ticket.Status == TicketStatus.Canceled)
                return false;

            ticket.Status = TicketStatus.Canceled;
            foreach (var seat in ticket.TicketSeats)
            {
                seat.SeatStatus = SeatStatus.Pending; // Revert seat status
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private string BuildTicketConfirmationEmail(CMTicket ticket)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<h1>Xác nhận vé của bạn</h1>");
            sb.AppendLine("<p>Thông tin vé:</p>");
            sb.AppendLine($"<p>Mã vé: {ticket.Id}</p>");
            sb.AppendLine($"<p>Trạng thái: {ticket.Status}</p>");
            sb.AppendLine($"<p>Thời gian chiếu: {ticket.Showtime.StartTime:dd/MM/yyyy HH:mm}</p>");
            sb.AppendLine($"<p>Tổng giá: {ticket.TotalPrice:C}</p>");
            sb.AppendLine("<p>Danh sách ghế:</p>");

            foreach (var ticketSeat in ticket.TicketSeats)
            {
                var seat = ticketSeat.Seat;
                sb.AppendLine($"<p>- Hàng: {seat.Row}, Số: {seat.Number}</p>");
            }

            sb.AppendLine("<p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>");
            return sb.ToString();
        }
    }

}
