using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Movie
{
    public class AddCommentDto
    {
        public int MovieId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }

        public IFormFile Image { get; set; }

    }
}