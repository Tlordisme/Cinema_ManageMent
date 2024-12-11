using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Movie
{
    public class MovieDto
    {
        public int Id { get; set; } // ID của phim
        public string Title { get; set; } // Tên phim
        public string Director { get; set; } // Đạo diễn
        public string Nation { get; set; } // Quốc gia
        public string Duration { get; set; } // Thời lượng (định dạng hh:mm:ss)
        public int LimitAge { get; set; } // Giới hạn độ tuổi
        public string Description { get; set; } // Mô tả phim
        public List<string> Genres { get; set; } // Danh sách thể loại
        public List<string> Casts { get; set; } // Danh sách diễn viên
    }

}
