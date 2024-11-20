using CM.Dtos.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Movie.Abstracts
{
    public interface ICommentService
    {
        Task AddCommentAsync(AddCommentDto commentdto, int userId);
        Task<IEnumerable<CommentDto>> GetCommentsByMovieId(int movieId);
    }
}