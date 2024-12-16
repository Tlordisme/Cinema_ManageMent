using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.Theater.Abstracts;
using CM.Dtos.Theater;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheaterController : ControllerBase
    {
        private readonly ITheaterService _theaterService;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<TheaterController> _logger;

        public TheaterController(ITheaterService theaterService, IPermissionService permissionService, ILogger<TheaterController> logger)
        {
            _theaterService = theaterService;
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpPost("AddTheater")]
        [Authorize]
        public IActionResult CreateTheater([FromBody] TheaterDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation("User {UserId} is attempting to create a new theater.", currentUserId);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.CreateTheater))
            {
                _logger.LogWarning("User {UserId} does not have permission to create theaters.", currentUserId);
                return Unauthorized("You do not have permission to create theaters.");
            }

            try
            {
                var id = _theaterService.CreateTheater(dto);
                _logger.LogInformation("Theater created successfully with ID {TheaterId}.", id);
                return Ok(new { Message = "Theater created successfully", TheaterId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating theater.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("GetTheaterByChainId/{chainId}")]
        [Authorize]
        public IActionResult GetTheatersByChainId(string chainId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation("User {UserId} is attempting to retrieve theaters for chain {ChainId}.", currentUserId, chainId);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewTheaters))
            {
                _logger.LogWarning("User {UserId} does not have permission to view theaters.", currentUserId);
                return Unauthorized("You do not have permission to view theaters.");
            }

            var theaters = _theaterService.GetTheatersByChainId(chainId);
            _logger.LogInformation("Retrieved {TheaterCount} theaters for chain {ChainId}.", theaters.Count(), chainId);
            return Ok(theaters);
        }

        [HttpDelete("DeleteTheater/{theaterId}")]
        [Authorize]
        public IActionResult DeleteTheater(string theaterId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation("User {UserId} is attempting to delete theater {TheaterId}.", currentUserId, theaterId);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.DeleteTheater))
            {
                _logger.LogWarning("User {UserId} does not have permission to delete theaters.", currentUserId);
                return Unauthorized("You do not have permission to delete theaters.");
            }

            try
            {
                _theaterService.DeleteTheater(theaterId);
                _logger.LogInformation("Theater {TheaterId} deleted successfully.", theaterId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting theater {TheaterId}.", theaterId);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateTheater/{theaterId}")]
        [Authorize]
        public IActionResult UpdateTheater([FromBody] TheaterDto dto, string theaterId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation("User {UserId} is attempting to update theater {TheaterId}.", currentUserId, theaterId);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateTheater))
            {
                _logger.LogWarning("User {UserId} does not have permission to update theaters.", currentUserId);
                return Unauthorized("You do not have permission to update theaters.");
            }

            try
            {
                var updatedId = _theaterService.UpdateTheater(dto);
                _logger.LogInformation("Theater {TheaterId} updated successfully with new ID {UpdatedId}.", theaterId, updatedId);
                return Ok(new { Id = updatedId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating theater {TheaterId}.", theaterId);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
