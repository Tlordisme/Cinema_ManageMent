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

        public TheaterController(ITheaterService theaterService, IPermissionService permissionService)
        {
            _theaterService = theaterService;
            _permissionService = permissionService;
        }

        [HttpPost("AddTheater")]
        [Authorize]
        public IActionResult CreateTheater([FromBody] TheaterDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.CreateTheater))
            {
                return Unauthorized("You do not have permission to create theaters.");
            }

            try
            {
                var id = _theaterService.CreateTheater(dto);
                return Ok(new { Message = "Theater created successfully", TheaterId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("GetTheaterByChainId/{chainId}")]
        [Authorize]
        public IActionResult GetTheatersByChainId(string chainId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewTheaters))
            {
                return Unauthorized("You do not have permission to view theaters.");
            }

            try
            {
                var theaters = _theaterService.GetTheatersByChainId(chainId);
                return Ok(theaters);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("DeleteTheater/{theaterId}")]
        [Authorize]
        public IActionResult DeleteTheater(string theaterId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.DeleteTheater))
            {
                return Unauthorized("You do not have permission to delete theaters.");
            }

            try
            {
                _theaterService.DeleteTheater(theaterId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("UpdateTheater/{theaterId}")]
        [Authorize]
        public IActionResult UpdateTheater([FromBody] TheaterDto dto, string theaterId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateTheater))
            {
                return Unauthorized("You do not have permission to update theaters.");
            }

            try
            {
                var updatedId = _theaterService.UpdateTheater(dto);
                return Ok(new { Id = updatedId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
