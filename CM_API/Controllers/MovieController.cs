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
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieService movieService, IPermissionService permissionService, ILogger<MovieController> logger)
        {
            _movieService = movieService;
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpPost("AddMovie")]
        [Authorize]
        public IActionResult AddMovie([FromBody] AddOrUpdateMovieDto movieDto)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation("User {UserId} bắt đầu thêm phim mới.", userId);

            // Kiểm tra quyền thêm phim
            if (!_permissionService.CheckPermission(userId, PermissionKey.AddMovie))
            {
                _logger.LogWarning("User {UserId} không có quyền thêm phim.", userId);
                return Unauthorized("You do not have permission to add a movie.");
            }

            if (movieDto == null)
            {
                _logger.LogWarning("Dữ liệu phim bị null khi User {UserId} thêm phim.", userId);
                return BadRequest("Movie data is null.");
            }

            try
            {
                _movieService.AddMovie(movieDto);
                _logger.LogInformation("Phim mới được thêm thành công bởi User {UserId}.", userId);
                return Ok("Movie added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi User {UserId} thêm phim.", userId);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateMovie/{movieId}")]
        [Authorize]
        public IActionResult UpdateMovie(int movieId, [FromBody] AddOrUpdateMovieDto movieDto)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation("User {UserId} bắt đầu cập nhật phim với ID {MovieId}.", userId, movieId);

            // Kiểm tra quyền cập nhật phim
            if (!_permissionService.CheckPermission(userId, PermissionKey.UpdateMovie))
            {
                _logger.LogWarning("User {UserId} không có quyền cập nhật phim với ID {MovieId}.", userId, movieId);
                return Unauthorized("You do not have permission to update a movie.");
            }

            if (movieDto == null)
            {
                _logger.LogWarning("Dữ liệu phim bị null khi User {UserId} cập nhật phim với ID {MovieId}.", userId, movieId);
                return BadRequest("Movie data is null.");
            }

            try
            {
                _movieService.UpdateMovie(movieId, movieDto);
                _logger.LogInformation("Phim với ID {MovieId} được cập nhật thành công bởi User {UserId}.", movieId, userId);
                return Ok("Movie updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi User {UserId} cập nhật phim với ID {MovieId}.", userId, movieId);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteMovie/{movieId}")]
        [Authorize]
        public IActionResult DeleteMovie(int movieId)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation("User {UserId} bắt đầu xóa phim với ID {MovieId}.", userId, movieId);

            // Kiểm tra quyền xóa phim
            if (!_permissionService.CheckPermission(userId, PermissionKey.DeleteMovie))
            {
                _logger.LogWarning("User {UserId} không có quyền xóa phim với ID {MovieId}.", userId, movieId);
                return Unauthorized("You do not have permission to delete a movie.");
            }

            try
            {
                _movieService.DeleteMovie(movieId);
                _logger.LogInformation("Phim với ID {MovieId} được xóa thành công bởi User {UserId}.", movieId, userId);
                return Ok("Movie deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi User {UserId} xóa phim với ID {MovieId}.", userId, movieId);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllMovies")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllMovies()
        {
            _logger.LogInformation("Bắt đầu lấy danh sách tất cả phim.");

            try
            {
                var movies = await _movieService.GetAllMovies();
                _logger.LogInformation("Danh sách phim được lấy thành công.");
                return Ok(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách tất cả phim.");
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}