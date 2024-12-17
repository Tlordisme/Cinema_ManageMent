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

        public ShowtimeController(IShowtimeService showtimeService, IPermissionService permissionService)
        {
            _showtimeService = showtimeService;
            _permissionService = permissionService;
        }

        [HttpPost("AddShowtime")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateShowtimeDto createShowtimeDto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.CreateShowtime))
            {
                return Unauthorized("You do not have permission to create showtimes.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newShowtime = await _showtimeService.CreateShowtimeAsync(createShowtimeDto);
                return CreatedAtAction(nameof(GetById), new { id = newShowtime.Id }, newShowtime);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllShowtimes")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewAllShowtimes))
            {
                return Unauthorized("You do not have permission to view all showtimes.");
            }

            var showtimes = await _showtimeService.GetAllShowtimesAsync();
            return Ok(showtimes);
        }

        [HttpGet("GetShowtime/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewShowtimeById))
            {
                return Unauthorized("You do not have permission to view this showtime.");
            }

            try
            {
                var showtime = await _showtimeService.GetShowtimeByIdAsync(id);
                return Ok(showtime);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateShowtime/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateShowtimeDto updateShowtimeDto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateShowtime))
            {
                return Unauthorized("You do not have permission to update showtimes.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updateShowtimeDto.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            try
            {
                var updated = await _showtimeService.UpdateShowtimeAsync(updateShowtimeDto);
                if (updated)
                {
                    return NoContent();
                }

                return NotFound(new { message = "Showtime not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteShowtime/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.DeleteShowtime))
            {
                return Unauthorized("You do not have permission to delete this showtime.");
            }

            try
            {
                var deleted = await _showtimeService.DeleteShowtimeAsync(id);
                if (deleted)
                {
                    return NoContent();
                }

                return NotFound(new { message = "Showtime not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
