using CM.Domain.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Ticket
{
    public class UpdateTicketDto
    {
        public int TicketId { get; set; }
        public TicketStatus Status { get; set; }
    }
}
