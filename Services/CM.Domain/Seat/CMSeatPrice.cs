using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Seat
{
    public class CMSeatPrice
    {
        public int Id { get; set; }

        public string SeatType { get; set; } // Loại ghế (Standard, VIP, ...)
        public string RoomID { get; set; } // ID của phòng

        public DateTime StartDate { get; set; } // Ngày bắt đầu áp dụng giá
        public DateTime EndDate { get; set; } // Ngày kết thúc áp dụng giá

        public decimal Price { get; set; } // Giá của ghế
    }
}
