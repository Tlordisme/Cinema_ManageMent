using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.Movie.Abstracts;
using CM.Dtos.Movie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IPermissionService _permissionService;

        public MovieController(IMovieService movieService, IPermissionService permissionService)
        {
            _movieService = movieService;
            _permissionService = permissionService;
        }

        [HttpPost("AddMovie")]
        [Authorize]
        public IActionResult AddMovie([FromBody] AddOrUpdateMovieDto movieDto)
        {
            if (!_permissionService.CheckPermission(GetUserId(), PermissionKey.AddMovie))
            {
                return Unauthorized("You do not have permission to add a movie.");
            }

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

        [HttpPut("UpdateMovie/{movieId}")]
        [Authorize]
        public IActionResult UpdateMovie(int movieId, [FromBody] AddOrUpdateMovieDto movieDto)
        {
            if (!_permissionService.CheckPermission(GetUserId(), PermissionKey.UpdateMovie))
            {
                return Unauthorized("You do not have permission to update a movie.");
            }

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

        [HttpDelete("DeleteMovie/{movieId}")]
        [Authorize]
        public IActionResult DeleteMovie(int movieId)
        {
            if (!_permissionService.CheckPermission(GetUserId(), PermissionKey.DeleteMovie))
            {
                return Unauthorized("You do not have permission to delete a movie.");
            }

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

        [HttpGet("GetAllMovies")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllMovies()
        {
            try
            {
                var movies = await _movieService.GetAllMovies();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst("Id")?.Value);
        }
    }
}