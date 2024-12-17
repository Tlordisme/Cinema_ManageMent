using CM.ApplicationService.RoleModule.Abstracts;
using CM.Auth.ApplicantService.Permission.Implements;
using CM.Domain.Auth;
using CM.Dtos.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class RoleController : Controller
{
    private readonly IRoleService _roleService;
    private readonly PermissionService _permissionService;

    public RoleController(IRoleService roleService, PermissionService permissionService)
    {
        _roleService = roleService;
        _permissionService = permissionService;
    }

    [HttpPost("CreateRole")]
    [Authorize]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(userId, "CreateRole"))
        {
            return Unauthorized("You do not have permission to create roles.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdRole = await _roleService.CreateRole(createRoleDto);
            return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, createdRole);
        }
        catch (Exception)
        {
            return BadRequest("An error occurred while creating the role.");
        }
    }

    [HttpGet("GetRole/{RoleId}")]
    [Authorize]
    public async Task<IActionResult> GetRoleById(int id)
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(userId, "GetRoleById"))
        {
            return Unauthorized("You do not have permission to view roles.");
        }

        try
        {
            var role = await _roleService.GetRoleById(id);
            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }
        catch (Exception)
        {
            return BadRequest("An error occurred while retrieving the role.");
        }
    }

    [HttpGet("GetAllRoles")]
    [Authorize]
    public async Task<IActionResult> GetAllRoles()
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(userId, "GetAllRoles"))
        {
            return Unauthorized("You do not have permission to view all roles.");
        }

        try
        {
            var roles = await _roleService.GetAllRoles();
            return Ok(roles);
        }
        catch (Exception)
        {
            return BadRequest("An error occurred while retrieving all roles.");
        }
    }

    [HttpPut("UpdateRole/{RoleId}")]
    [Authorize]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(userId, "UpdateRole"))
        {
            return Unauthorized("You do not have permission to update roles.");
        }

        try
        {
            var updatedRole = await _roleService.UpdateRole(updateRoleDto);
            return Ok(updatedRole);
        }
        catch (Exception)
        {
            return BadRequest("An error occurred while updating the role.");
        }
    }

    [HttpDelete("DeleteRole/{RoleId}")]
    [Authorize]
    public async Task<IActionResult> DeleteRole(int roleId)
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(userId, "DeleteRole"))
        {
            return Unauthorized("You do not have permission to delete roles.");
        }

        try
        {
            var result = await _roleService.DeleteRole(roleId);
            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }
        catch (Exception)
        {
            return BadRequest("An error occurred while deleting the role.");
        }
    }

    [HttpPut("AddRoleToUser/{userId}")]
    [Authorize]
    public async Task<IActionResult> AddRoleToUser(int userId, [FromBody] string role)
    {
        var userIdClaim = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(userIdClaim, "AddRoleToUser"))
        {
            return Unauthorized("You do not have permission to assign roles to users.");
        }

        try
        {
            var result = await _roleService.AddRoleToUser(userId, role);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest("An error occurred while assigning the role.");
        }
    }
}
