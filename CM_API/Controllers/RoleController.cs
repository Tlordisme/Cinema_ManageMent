using CM.ApplicationService.RoleModule.Abstracts;
using CM.Auth.ApplicantService.Permission.Implements;
using CM.Dtos.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly PermissionService _permissionService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, PermissionService permissionService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpPost("CreateRole")]
        [Authorize]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);

            // Kiểm tra quyền tạo vai trò
            if (!_permissionService.CheckPermission(userId, "CreateRole"))
            {
                _logger.LogWarning($"User {userId} attempted to create a role without permission.");
                return Unauthorized("You do not have permission to create roles.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"User {userId} submitted invalid model for role creation.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdRole = await _roleService.CreateRole(createRoleDto);
                _logger.LogInformation($"Role {createdRole.Id} created successfully by user {userId}.");
                return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, createdRole);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating role: {ex.Message}");
                return BadRequest("An error occurred while creating the role.");
            }
        }

        [HttpGet("GetRole/{RoleId}")]
        [Authorize]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);

            // Kiểm tra quyền lấy thông tin vai trò
            if (!_permissionService.CheckPermission(userId, "GetRoleById"))
            {
                _logger.LogWarning($"User {userId} attempted to access role {id} without permission.");
                return Unauthorized("You do not have permission to view roles.");
            }

            try
            {
                var role = await _roleService.GetRoleById(id);
                if (role == null)
                {
                    _logger.LogWarning($"Role {id} not found for user {userId}.");
                    return NotFound();
                }

                _logger.LogInformation($"User {userId} retrieved role {id}.");
                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving role {id}: {ex.Message}");
                return BadRequest("An error occurred while retrieving the role.");
            }
        }

        [HttpGet("GetAllRoles")]
        [Authorize]
        public async Task<IActionResult> GetAllRoles()
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);

            // Kiểm tra quyền lấy tất cả vai trò
            if (!_permissionService.CheckPermission(userId, "GetAllRoles"))
            {
                _logger.LogWarning($"User {userId} attempted to retrieve all roles without permission.");
                return Unauthorized("You do not have permission to view all roles.");
            }

            try
            {
                var roles = await _roleService.GetAllRoles();
                _logger.LogInformation($"User {userId} retrieved all roles.");
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving all roles: {ex.Message}");
                return BadRequest("An error occurred while retrieving all roles.");
            }
        }

        [HttpPut("UpdateRole/{RoleId}")]
        [Authorize]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);

            // Kiểm tra quyền cập nhật vai trò
            if (!_permissionService.CheckPermission(userId, "UpdateRole"))
            {
                _logger.LogWarning($"User {userId} attempted to update a role without permission.");
                return Unauthorized("You do not have permission to update roles.");
            }

            try
            {
                var updatedRole = await _roleService.UpdateRole(updateRoleDto);
                _logger.LogInformation($"Role {updateRoleDto.Id} updated successfully by user {userId}.");
                return Ok(updatedRole);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating role {updateRoleDto.Id}: {ex.Message}");
                return BadRequest("An error occurred while updating the role.");
            }
        }

        [HttpDelete("DeleteRole/{RoleId}")]
        [Authorize]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);

            // Kiểm tra quyền xóa vai trò
            if (!_permissionService.CheckPermission(userId, "DeleteRole"))
            {
                _logger.LogWarning($"User {userId} attempted to delete role {roleId} without permission.");
                return Unauthorized("You do not have permission to delete roles.");
            }

            try
            {
                var result = await _roleService.DeleteRole(roleId);
                if (result)
                {
                    _logger.LogInformation($"Role {roleId} deleted successfully by user {userId}.");
                    return NoContent();
                }

                _logger.LogWarning($"Role {roleId} not found for deletion by user {userId}.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting role {roleId}: {ex.Message}");
                return BadRequest("An error occurred while deleting the role.");
            }
        }

        [HttpPut("AddRoleToUser/{userId}")]
        [Authorize]
        public async Task<IActionResult> AddRoleToUser(int userId, [FromBody] string role)
        {
            var userIdClaim = int.Parse(User.FindFirst("Id")?.Value);

            // Kiểm tra quyền gán vai trò cho người dùng
            if (!_permissionService.CheckPermission(userIdClaim, "AddRoleToUser"))
            {
                _logger.LogWarning($"User {userIdClaim} attempted to assign role {role} to user {userId} without permission.");
                return Unauthorized("You do not have permission to assign roles to users.");
            }

            try
            {
                var result = await _roleService.AddRoleToUser(userId, role);
                _logger.LogInformation($"User {userIdClaim} assigned role {role} to user {userId}.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error assigning role {role} to user {userId}: {ex.Message}");
                return BadRequest("An error occurred while assigning the role to the user.");
            }
        }
    }
}