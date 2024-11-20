using CM.Domain.Movie;
using CM.Domain.Theater;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Showtime
{
    public class CMShowtime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RoomID { get; set; }

        public CMRoom Room { get; set; }
        public int MovieID { get; set; }
        public MoMovie Movie { get; set; }
    }
}
