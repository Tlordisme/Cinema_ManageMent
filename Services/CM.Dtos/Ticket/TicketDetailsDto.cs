using CM.Dtos.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Ticket
{
    public class TicketDetailsDto
    {
        public int TicketId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime Showtime { get; set; }
        public string MovieName { get; set; }
        public string TheaterName { get; set; }
        public string RoomName { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<SeatForTicketDto> Seats { get; set; }
    }

}
