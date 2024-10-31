using CM.Movie.ApplicantService.MovieModule.Abstracts;
using CM.Movie.Domain;
using CM.Movie.Dtos.MovieModule;
using CM.Movie.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Share.ApplicationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Movie.ApplicantService.MovieModule.Implements
{
    public class MovieService :  BaseService, IMovieService
    {
        private readonly MovieDbContext _context;

        public MovieService(MovieDbContext context, ILogger<MovieService> logger) : base(logger)
        {
            _context = context;
        }
        public void AddMovie(AddOrUpdateMovieDto movieDto)
        {
            var movie = new MoMovie
            {
                Title = movieDto.Title,
                Director = movieDto.Director,
                Country = movieDto.Country,
                Language = movieDto.Language
            };

            // Thêm các thể loại mới hoặc liên kết thể loại
            foreach (var genreName in movieDto.Genres)
            {
                var genre = _context.Genres.FirstOrDefault(g => g.Name == genreName);
                if (genre == null)
                {
                    genre = new MoGenre { Name = genreName };
                    _context.Genres.Add(genre);
                    LogInformation($"Added new genre: {genreName}");
                }
                movie.MovieGenres.Add(genre);
            }

            // Thêm các diễn viên mới hoặc liên kết diễn viên
            foreach (var castName in movieDto.Casts)
            {
                var cast = _context.Casts.FirstOrDefault(c => c.Name == castName);
                if (cast == null)
                {
                    cast = new MoCast { Name = castName };
                    _context.Casts.Add(cast);
                    LogInformation($"Added new cast: {castName}");
                }
                //movie.MovieCasts.Add(cast);
            }

            _context.Movies.Add(movie);
            _context.SaveChanges();
            LogInformation($"Added movie: {movie.Title}");
        }

        public void UpdateMovie(int movieId, AddOrUpdateMovieDto movieDto)
        {
            var movie = _context.Movies
                .Include(m => m.MovieGenres)
                .Include(m => m.MovieCasts)
                .FirstOrDefault(m => m.Id == movieId);

            if (movie == null)
            {
                LogWarning($"Attempted to update movie with ID {movieId}, but it was not found.");
                throw new Exception("Movie not found");
            }

            movie.Title = movieDto.Title;
            movie.Director = movieDto.Director;
            movie.Country = movieDto.Country;
            movie.Language = movieDto.Language;

            // Cập nhật lại các thể loại
            movie.MovieGenres.Clear();
            foreach (var genreName in movieDto.Genres)
            {
                var genre = _context.Genres.FirstOrDefault(g => g.Name == genreName) ?? new MoGenre { Name = genreName };
                movie.MovieGenres.Add(genre);
            }

            // Cập nhật lại các diễn viên
            movie.MovieCasts.Clear();
            foreach (var castName in movieDto.Casts)
            {
                var cast = _context.Casts.FirstOrDefault(c => c.Name == castName) ?? new MoCast { Name = castName };
                movie.MovieCasts.Add(cast);
            }

            _context.SaveChanges();
            LogInformation($"Updated movie: {movie.Title}");
        }

        public void DeleteMovie(int movieId)
        {
            var movie = _context.Movies.Find(movieId);
            if (movie == null)
            {
                LogWarning($"Attempted to delete movie with ID {movieId}, but it was not found.");
                throw new Exception("Movie not found");
            }

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            LogInformation($"Deleted movie: {movie.Title}");
        }
    }
}
