using CM.Domain.Ticket;
using CM.Dtos.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Ticket
{
    public class TicketDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ShowtimeId { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<TicketSeatDto> Seats { get; set; }
    }
}
