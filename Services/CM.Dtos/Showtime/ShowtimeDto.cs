using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Showtime
{
    public class ShowtimeDto
    {
        public string Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RoomId { get; set; }
        public string RoomName { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
    }

}
