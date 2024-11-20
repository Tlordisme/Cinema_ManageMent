using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Movie
{
    public class AddOrUpdateMovieDto
    {
        public string Title { get; set; }
        public string Director { get; set; }
        public string Duration { get; set; }
        public string Nation { get; set; }
        public DateOnly PublicTime { get; set; }
        public int LimitAge { get; set; }
        public string Description { get; set; }
        public List<string> Genres { get; set; } = new List<string>(); // Danh sách tên thể loại
        public List<string> Casts { get; set; } = new List<string>(); // Danh sách tên diễn viên
    }

}
