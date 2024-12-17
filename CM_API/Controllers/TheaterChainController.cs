using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.Theater.Abstracts;
using CM.Domain.Auth;
using CM.Dtos.Theater;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;

[ApiController]
[Route("api/[controller]")]
public class TheaterChainController : Controller
{
    private readonly ITheaterChainService _theaterChainService;
    private readonly IPermissionService _permissionService;

    public TheaterChainController(ITheaterChainService theaterChainService, IPermissionService permissionService)
    {
        _theaterChainService = theaterChainService;
        _permissionService = permissionService;
    }

    [HttpPost("CreateTheaterChain")]
    [Authorize]
    public IActionResult CreateTheaterChain([FromBody] TheaterChainDto dto)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(currentUserId, PermissionKey.CreateTheaterChain))
        {
            return Unauthorized("You do not have permission to create a theater chain.");
        }

        try
        {
            var id = _theaterChainService.CreateTheaterChain(dto);
            return Ok(new { Message = "TheaterChain created successfully", TheaterChain = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("GetAllTheaterChains")]
    [Authorize]
    public IActionResult GetAllTheaterChains()
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewAllTheaterChains))
        {
            return Unauthorized("You do not have permission to view all theater chains.");
        }

        var theaterChains = _theaterChainService.GetAllTheaterChains();
        return Ok(theaterChains);
    }

    [HttpDelete("DeleteTheaterChain/{theaterChainId}")]
    [Authorize]
    public IActionResult DeleteTheaterChain(string theaterChainId)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(currentUserId, PermissionKey.DeleteTheaterChain))
        {
            return Unauthorized("You do not have permission to delete this theater chain.");
        }

        try
        {
            _theaterChainService.DeleteTheaterChain(theaterChainId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("UpdateTheaterChain/{theaterChainId}")]
    [Authorize]
    public IActionResult UpdateTheaterChain([FromBody] TheaterChainDto dto)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateTheaterChain))
        {
            return Unauthorized("You do not have permission to update this theater chain.");
        }

        try
        {
            var updatedId = _theaterChainService.UpdateTheaterChain(dto);
            return Ok(new { Id = updatedId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
