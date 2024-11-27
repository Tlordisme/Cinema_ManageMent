using CM.Domain.Seat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Ticket
{
    public class CMTicketSeat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Mối quan hệ với vé
        public int TicketId { get; set; }  // Mã vé
        public CMTicket Ticket { get; set; }  // Vé mà ghế này thuộc về

        // Mối quan hệ với ghế
        public int SeatId { get; set; }  // Mã ghế
        public CMSeat Seat { get; set; }  // Ghế mà vé này áp dụng

        // Trạng thái ghế: đã thanh toán, chưa thanh toán...
        public SeatStatus SeatStatus { get; set; }
    }

    public enum SeatStatus
    {
        Pending,    // Ghế chưa thanh toán
        Paid,       // Ghế đã thanh toán
        Reserved    // Ghế đã được đặt trước
    }

}
