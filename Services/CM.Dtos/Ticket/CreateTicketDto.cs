using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Ticket
{
    public class CreateTicketDto
    {
        public int UserId { get; set; }
        public string ShowtimeId { get; set; }
        //public List<(string Row, int Number)> Seats { get; set; } 
        //public decimal TotalPrice { get; set; }
    }
}
