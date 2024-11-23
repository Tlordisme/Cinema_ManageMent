using CM.Domain.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Ticket
{
    public class TicketSeatDto
    {
        public int SeatId { get; set; }
        public SeatStatus SeatStatus { get; set; }
    }
}
