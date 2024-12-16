using CM.ApplicationService.Theater.Abstracts;
using CM.Dtos.Theater;
using CM.ApplicantService.Auth.Permission.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;

namespace CM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheaterChainController : Controller
    {
        private readonly ITheaterChainService _theaterChainService;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<TheaterChainController> _logger;

        public TheaterChainController(ITheaterChainService theaterChainService, IPermissionService permissionService, ILogger<TheaterChainController> logger)
        {
            _theaterChainService = theaterChainService;
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpPost("CreateTheaterChain")]
        [Authorize]
        public IActionResult CreateTheaterChain([FromBody] TheaterChainDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation("User {UserId} is attempting to create a new theater chain.", currentUserId);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.CreateTheaterChain))
            {
                _logger.LogWarning("User {UserId} does not have permission to create a theater chain.", currentUserId);
                return Unauthorized("You do not have permission to create a theater chain.");
            }

            try
            {
                var id = _theaterChainService.CreateTheaterChain(dto);
                _logger.LogInformation("Theater chain created successfully with ID {TheaterChainId}.", id);
                return Ok(new { Message = "TheaterChain created successfully", TheaterChain = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating theater chain.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllTheaterChains")]
        [Authorize]
        public IActionResult GetAllTheaterChains()
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation("User {UserId} is attempting to retrieve all theater chains.", currentUserId);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewAllTheaterChains))
            {
                _logger.LogWarning("User {UserId} does not have permission to view all theater chains.", currentUserId);
                return Unauthorized("You do not have permission to view all theater chains.");
            }

            var theaterChains = _theaterChainService.GetAllTheaterChains();
            _logger.LogInformation("Retrieved {TheaterChainCount} theater chains.", theaterChains.Count);
            return Ok(theaterChains);
        }

        [HttpDelete("DeleteTheaterChain/{theaterChainId}")]
        [Authorize]
        public IActionResult DeleteTheaterChain(string theaterChainId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation("User {UserId} is attempting to delete theater chain {TheaterChainId}.", currentUserId, theaterChainId);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.DeleteTheaterChain))
            {
                _logger.LogWarning("User {UserId} does not have permission to delete theater chain {TheaterChainId}.", currentUserId, theaterChainId);
                return Unauthorized("You do not have permission to delete this theater chain.");
            }

            try
            {
                _theaterChainService.DeleteTheaterChain(theaterChainId);
                _logger.LogInformation("Theater chain {TheaterChainId} deleted successfully.", theaterChainId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting theater chain {TheaterChainId}.", theaterChainId);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateTheaterChain/{theaterChainId}")]
        [Authorize]
        public IActionResult UpdateTheaterChain([FromBody] TheaterChainDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            _logger.LogInformation("User {UserId} is attempting to update theater chain.", currentUserId);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateTheaterChain))
            {
                _logger.LogWarning("User {UserId} does not have permission to update theater chain.", currentUserId);
                return Unauthorized("You do not have permission to update this theater chain.");
            }

            try
            {
                var updatedId = _theaterChainService.UpdateTheaterChain(dto);
                _logger.LogInformation("Theater chain updated successfully with new ID {UpdatedId}.", updatedId);
                return Ok(new { Id = updatedId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating theater chain.");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
