using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Movie.Dtos.MovieModule
{
    public class AddOrUpdateMovieDto
    {
        public string Title { get; set; }
        public string Director { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public List<string> Genres { get; set; } = new List<string>(); // Danh sách tên thể loại
        public List<string> Casts { get; set; } = new List<string>(); // Danh sách tên diễn viên
    }

}
