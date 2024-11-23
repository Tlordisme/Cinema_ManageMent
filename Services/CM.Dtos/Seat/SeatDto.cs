using CM.Domain.Seat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Seat
{
    public class SeatDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string SeatType { get; set; }
        public string RoomID { get; set; }
        public SeatStatus Status { get; set; }
        public bool IsDoubleSeat { get; set; }
        public int? DoubleSeatId { get; set; }

    }
}
