using CM.Movie.Dtos.MovieModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Movie.ApplicantService.MovieModule.Abstracts
{
    public interface IMovieService
    {
        void AddMovie(AddOrUpdateMovieDto movieDto);
        void UpdateMovie(int movieId, AddOrUpdateMovieDto movieDto);
        void DeleteMovie(int movieId);
    }
}
