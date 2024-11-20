using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Movie
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }
        public int UserId { get; set; }
    }
}