using CM.Domain.Auth;
using CM.Domain.Seat;
using CM.Domain.Showtime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Ticket
{
    public class CMTicket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Thông tin người mua vé
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        // Lịch chiếu của phim
        public string ShowtimeId { get; set; }
        public CMShowtime Showtime { get; set; }

        // Trạng thái vé: Đang chờ thanh toán, Đã thanh toán, Đã hủy
        public TicketStatus Status { get; set; }
        public DateTime BookingDate { get; set; }

        // Tổng giá vé (bao gồm ghế, combo, ...)
        public decimal TotalPrice { get; set; }
        public List<CMTicketSeat> TicketSeats { get; set; }

    }
    public enum TicketStatus
    {
        Pending,   
        Paid,      
        Canceled,   
        Used       
    }
}
