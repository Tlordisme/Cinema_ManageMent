using CM.Domain.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Ticket
{
    public class CMTicketSeat
    {
        public int Id { get; set; }
        public CMTicket Ticket { get; set; }
        public int TicketId { get; set; }
        public int SeatId { get; set; }
        public CMSeat Seat { get; set; }

        // Trạng thái ghế: đã thanh toán, chưa thanh toán...
        public SeatStatus SeatStatus { get; set; }
    }
}
