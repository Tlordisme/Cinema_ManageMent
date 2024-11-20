using CM.Domain.Showtime;
using CM.Domain.Theater;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Seat
{
    public class CMSeat
    {
        [Key]
        public int Id { get; set; }

        public int Number { get; set; } 
        
        public int Row { get; set; }

        public string SeatType { get; set; }    
        public decimal Price { get; set; } 


        public string RoomID { get; set; }
        public CMRoom Room { get; set; }

        public SeatStatus Status { get; set; }
    }
    public enum SeatStatus
    {
        Available,   // Ghế còn trống
        Reserved,    // Ghế đã được tạm giữ (chưa thanh toán)
        Booked       // Ghế đã được thanh toán và không thể thay đổi
    }
}
