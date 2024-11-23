using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Seat
{
    public class AddSeatPriceDto
    {
        public string SeatType { get; set; } // Loại ghế
        public string RoomID { get; set; } // ID của phòng
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; } // Ngày kết thúc
        public decimal Price { get; set; } // Giá
    }
}
