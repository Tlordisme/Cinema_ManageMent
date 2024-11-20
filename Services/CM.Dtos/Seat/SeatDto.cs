using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Seat
{
    public class SeatDto
    {
        public int Id { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public string SeatType { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }
}
