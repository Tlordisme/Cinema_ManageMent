using CM.ApplicationService.Showtime.Abstracts;
using CM.ApplicationService.Showtime.Implements;
using CM.Dtos.Showtime;
using CM.ApplicantService.Auth.Permission.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeController : Controller
    {
        private readonly IShowtimeService _showtimeService;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<ShowtimeController> _logger; 

        public ShowtimeController(IShowtimeService showtimeService, IPermissionService permissionService, ILogger<ShowtimeController> logger)
        {
            _showtimeService = showtimeService;
            _permissionService = permissionService;
            _logger = logger; 
        }

        [HttpPost("AddShowtime")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateShowtimeDto createShowtimeDto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation($"User {currentUserId} is attempting to create a new showtime.");

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.CreateShowtime))
            {
                _logger.LogWarning($"User {currentUserId} does not have permission to create a showtime.");
                return Unauthorized("You do not have permission to create showtimes.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Model state is invalid for user {currentUserId} while creating showtime.");
                return BadRequest(ModelState);
            }

            try
            {
                var newShowtime = await _showtimeService.CreateShowtimeAsync(createShowtimeDto);
                _logger.LogInformation($"Showtime with ID {newShowtime.Id} created successfully.");
                return CreatedAtAction(nameof(GetById), new { id = newShowtime.Id }, newShowtime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new showtime.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllShowtimes")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation($"User {currentUserId} is attempting to view all showtimes.");

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewAllShowtimes))
            {
                _logger.LogWarning($"User {currentUserId} does not have permission to view all showtimes.");
                return Unauthorized("You do not have permission to view all showtimes.");
            }

            var showtimes = await _showtimeService.GetAllShowtimesAsync();
            _logger.LogInformation($"Successfully retrieved {showtimes.Count()} showtimes.");
            return Ok(showtimes);
        }

        [HttpGet("GetShowtime/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation($"User {currentUserId} is attempting to view showtime with ID {id}.");

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewShowtimeById))
            {
                _logger.LogWarning($"User {currentUserId} does not have permission to view showtime with ID {id}.");
                return Unauthorized("You do not have permission to view this showtime.");
            }

            try
            {
                var showtime = await _showtimeService.GetShowtimeByIdAsync(id);
                if (showtime == null)
                {
                    _logger.LogWarning($"Showtime with ID {id} not found.");
                    return NotFound(new { message = "Showtime not found" });
                }

                _logger.LogInformation($"Successfully retrieved showtime with ID {id}.");
                return Ok(showtime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching showtime with ID {id}.");
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateShowtime/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateShowtimeDto updateShowtimeDto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation($"User {currentUserId} is attempting to update showtime with ID {id}.");

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateShowtime))
            {
                _logger.LogWarning($"User {currentUserId} does not have permission to update showtime with ID {id}.");
                return Unauthorized("You do not have permission to update showtimes.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Model state is invalid for user {currentUserId} while updating showtime with ID {id}.");
                return BadRequest(ModelState);
            }

            if (id != updateShowtimeDto.Id)
            {
                _logger.LogWarning($"ID mismatch: {id} != {updateShowtimeDto.Id}.");
                return BadRequest(new { message = "ID mismatch" });
            }

            try
            {
                var updated = await _showtimeService.UpdateShowtimeAsync(updateShowtimeDto);
                if (updated)
                {
                    _logger.LogInformation($"Successfully updated showtime with ID {id}.");
                    return NoContent();
                }

                _logger.LogWarning($"Showtime with ID {id} not found.");
                return NotFound(new { message = "Showtime not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating showtime with ID {id}.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteShowtime/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation($"User {currentUserId} is attempting to delete showtime with ID {id}.");

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.DeleteShowtime))
            {
                _logger.LogWarning($"User {currentUserId} does not have permission to delete showtime with ID {id}.");
                return Unauthorized("You do not have permission to delete this showtime.");
            }

            try
            {
                var deleted = await _showtimeService.DeleteShowtimeAsync(id);
                if (deleted)
                {
                    _logger.LogInformation($"Successfully deleted showtime with ID {id}.");
                    return NoContent();
                }

                _logger.LogWarning($"Showtime with ID {id} not found.");
                return NotFound(new { message = "Showtime not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting showtime with ID {id}.");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}