//using CM.Domain.Auth;
//using CM.Domain.FoodCombo;
//using CM.Domain.Seat;
//using CM.Domain.Showtime;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CM.Domain.Ticket
//{
//    public class CMTicket
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }

//        // Thông tin người mua vé
//        public int UserId { get; set; }
//        public User User { get; set; }

//        // Lịch chiếu của phim
//        public string ShowtimeId { get; set; }
//        public CMShowtime Showtime { get; set; }

//        // Trạng thái vé: Đang chờ thanh toán, Đã thanh toán, Đã hủy
//        public TicketStatus Status { get; set; }

//        // Ngày tạo vé
//        public DateTime CreatedDate { get; set; }

//        // Tổng giá vé (bao gồm ghế, combo, ...)
//        public decimal TotalPrice { get; set; }

//        // Danh sách ghế trong vé này
//        public List<CMTicketSeat> TicketSeats { get; set; } = new List<TicketSeat>();

//    }
//    public enum TicketStatus
//    {
//        Pending,    // Đang chờ thanh toán
//        Paid,       // Đã thanh toán
//        Canceled,   // Đã hủy
//        Used        // Đã sử dụng
//    }
//}
