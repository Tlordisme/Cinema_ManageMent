using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.ApplicationService.Common;
using CM.ApplicationService.Movie.Abstracts;
using CM.Domain.Movie;
using CM.Dtos.Movie;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Share.ApplicationService;

namespace CM.ApplicationService.Movie.Implements
{
    public class MovieService : ServiceBase, IMovieService
    {
        public MovieService(CMDbContext dbcontext, ILogger<MovieService> logger)
            : base(logger, dbcontext) { }

        public void AddMovie(AddOrUpdateMovieDto movieDto)
        {
            TimeSpan duration;
            try
            {
                duration = TimeSpan.Parse(movieDto.Duration);
            }
            catch (FormatException)
            {
                throw new Exception("Invalid duration format. Please use 'hh:mm:ss'.");
            }
            var movie = new MoMovie
            {
                Title = movieDto.Title,
                Director = movieDto.Director,
                Duration = duration,
                Nation = movieDto.Nation,
                LimitAge = movieDto.LimitAge,
                Description = movieDto.Description,
                MovieGenres = new List<MoMovie_Genre>(),
                MovieCasts = new List<MoMovie_Cast>(),
            };

            // Thêm các thể loại mới hoặc liên kết thể loại
            foreach (var genreName in movieDto.Genres)
            {
                var genre = _dbContext.Genres.FirstOrDefault(g => g.Name == genreName);
                if (genre == null)
                {
                    genre = new MoGenre { Name = genreName };
                    _dbContext.Genres.Add(genre);
                    _logger.LogInformation($"Added new genre: {genreName}");
                }
                movie.MovieGenres.Add(new MoMovie_Genre { Genre = genre });
            }

            // Thêm các diễn viên mới hoặc liên kết diễn viên
            foreach (var castName in movieDto.Casts)
            {
                var cast = _dbContext.Casts.FirstOrDefault(c => c.Name == castName);
                if (cast == null)
                {
                    cast = new MoCast { Name = castName };
                    _dbContext.Casts.Add(cast);
                    _logger.LogInformation($"Added new cast: {castName}");
                }
                movie.MovieCasts.Add(new MoMovie_Cast { Cast = cast });
            }

            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
            _logger.LogInformation($"Added movie: {movie.Title}");
        }

        public void UpdateMovie(int movieId, AddOrUpdateMovieDto movieDto)
        {
            var movie = _dbContext
                .Movies.Include(m => m.MovieGenres)
                .Include(m => m.MovieCasts)
                .FirstOrDefault(m => m.Id == movieId);

            if (movie == null)
            {
                _logger.LogWarning($"Attempted to update movie with ID {movieId}, but it was not found.");
                throw new Exception("Movie not found");
            }
            TimeSpan duration;
            try
            {
                duration = TimeSpan.Parse(movieDto.Duration); // Chuyển đổi từ chuỗi sang TimeSpan
            }
            catch (FormatException)
            {
                throw new Exception("Invalid duration format. Please use 'hh:mm:ss'.");
            }

            movie.Title = movieDto.Title;
            movie.Director = movieDto.Director;
            movie.Nation = movieDto.Nation;
            movie.Duration = duration;


            // Cập nhật lại các thể loại
            movie.MovieGenres.Clear();
            foreach (var genreName in movieDto.Genres)
            {
                var genre =
                    _dbContext.Genres.FirstOrDefault(g => g.Name == genreName)
                    ?? new MoGenre { Name = genreName };
                movie.MovieGenres.Add(new MoMovie_Genre { Genre = genre });
            }

            // Cập nhật lại các diễn viên
            movie.MovieCasts.Clear();
            movie.MovieCasts.Clear();
            foreach (var castName in movieDto.Casts)
            {
                var cast =
                    _dbContext.Casts.FirstOrDefault(c => c.Name == castName)
                    ?? new MoCast { Name = castName };
                movie.MovieCasts.Add(new MoMovie_Cast { Cast = cast });
            }

            _dbContext.SaveChanges();
            _logger.LogInformation($"Updated movie: {movie.Title}");
        }

        public void DeleteMovie(int movieId)
        {
            var movie = _dbContext.Movies.Find(movieId);
            if (movie == null)
            {
                _logger.LogWarning($"Attempted to delete movie with ID {movieId}, but it was not found.");
                throw new Exception("Movie not found");
            }

            _dbContext.Movies.Remove(movie);
            _dbContext.SaveChanges();
            _logger.LogInformation($"Deleted movie: {movie.Title}");
        }

        public async Task<IEnumerable<MovieDto>> GetAllMovies()
        {
            try
            {
                var movies = await _dbContext.Movies
                    .Include(m => m.MovieGenres)
                        .ThenInclude(mg => mg.Genre)
                    .Include(m => m.MovieCasts)
                        .ThenInclude(mc => mc.Cast)
                    .Select(m => new MovieDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Director = m.Director,
                        Nation = m.Nation,
                        Duration = m.Duration.ToString(@"hh\:mm\:ss"),
                        LimitAge = m.LimitAge,
                        Description = m.Description,
                        Genres = m.MovieGenres.Select(g => g.Genre.Name).ToList(),
                        Casts = m.MovieCasts.Select(c => c.Cast.Name).ToList()
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {movies.Count} movies from the database.");
                return movies;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving movies: {ex.Message}", ex);
                throw;
            }
        }
    }
}
