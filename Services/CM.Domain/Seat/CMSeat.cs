using CM.Domain.Showtime;
using CM.Domain.Theater;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Seat
{
    public class CMSeat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(4)]
        public string Name { get; set; }

        [Range(1, 100)]
        public int X { get; set; }

        [Range(1, 100)]
        public int Y { get; set; }

        [MaxLength(20)]
        public string SeatType { get; set; } 

        [Required]
        public string RoomID { get; set; }
        public CMRoom Room { get; set; }

        [Required]
        public SeatStatus Status { get; set; }


        public int? DoubleSeatId { get; set; }  
    }

    public enum SeatStatus
    {
        Available,
        Pending,
        Booked
    }
}
