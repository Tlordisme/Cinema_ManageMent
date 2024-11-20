using CM.ApplicationService.Movie.Abstracts;
using CM.Dtos.Movie;

using Microsoft.AspNetCore.Mvc;


namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost]
        public IActionResult AddMovie([FromBody] AddOrUpdateMovieDto movieDto)
        {
            if (movieDto == null)
            {
                return BadRequest("Movie data is null.");
            }

            try
            {
                _movieService.AddMovie(movieDto);
                return Ok("Movie added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{movieId}")]
        public IActionResult UpdateMovie(int movieId, [FromBody] AddOrUpdateMovieDto movieDto)
        {
            if (movieDto == null)
            {
                return BadRequest("Movie data is null.");
            }

            try
            {
                _movieService.UpdateMovie(movieId, movieDto);
                return Ok("Movie updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{movieId}")]
        public IActionResult DeleteMovie(int movieId)
        {
            try
            {
                _movieService.DeleteMovie(movieId);
                return Ok("Movie deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}