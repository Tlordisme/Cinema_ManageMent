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

        public RoleController(IRoleService roleService, PermissionService permissionService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
        }

        // Tạo vai trò mới
        [HttpPost("Createroles")]
        [Authorize]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);

            // Kiểm tra quyền "CreateRole"
            if (!_permissionService.CheckPermission(userId, "CreateRole"))
            {
                return Unauthorized("You do not have permission to create roles.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdRole = await _roleService.CreateRole(createRoleDto);
            return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, createdRole);
        }

        // Lấy thông tin vai trò theo ID
        [HttpGet("Getroles/{id}")]
        [Authorize]

        public async Task<IActionResult> GetRoleById(int id)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value); // Lấy userId từ JWT claim

            // Kiểm tra quyền "ViewRole"
            if (!_permissionService.CheckPermission(userId, "GetRoleById"))
            {
                return Unauthorized("You do not have permission to view roles.");
            }

            var role = await _roleService.GetRoleById(id);
            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        // Lấy tất cả vai trò
        [HttpGet("GetAllroles")]
        [Authorize]
        public async Task<IActionResult> GetAllRoles()
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value); // Lấy userId từ JWT claim

            // Kiểm tra quyền "ViewAllRoles"
            if (!_permissionService.CheckPermission(userId, "GetAllRoles"))
            {
                return Unauthorized("You do not have permission to view all roles.");
            }

            var roles = await _roleService.GetAllRoles();
            return Ok(roles);
        }

        // Cập nhật vai trò
        [HttpPut("Updateroles")]
        [Authorize]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value); // Lấy userId từ JWT claim

            // Kiểm tra quyền "UpdateRole"
            if (!_permissionService.CheckPermission(userId, "UpdateRole"))
            {
                return Unauthorized("You do not have permission to update roles.");
            }

            var updatedRole = await _roleService.UpdateRole(updateRoleDto);
            return Ok(updatedRole);
        }

        // Xóa vai trò
        [HttpDelete("Deleteroles/{roleId}")]
        [Authorize]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value); // Lấy userId từ JWT claim

            // Kiểm tra quyền "DeleteRole"
            if (!_permissionService.CheckPermission(userId, "DeleteRole"))
            {
                return Unauthorized("You do not have permission to delete roles.");
            }

            var result = await _roleService.DeleteRole(roleId);
            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }

        // Gán vai trò cho người dùng
        [HttpPut("AddRoletouser/{userId}")]
        [Authorize]
        public async Task<IActionResult> AddRoleToUser(int userId, [FromBody] string role)
        {
            var userIdClaim = int.Parse(User.FindFirst("Id")?.Value); // Lấy userId từ JWT claim

            // Kiểm tra quyền "AssignRoleToUser"
            if (!_permissionService.CheckPermission(userIdClaim, "AddRoleToUser"))
            {
                return Unauthorized("You do not have permission to assign roles to users.");
            }

            var result = await _roleService.AddRoleToUser(userId, role);
            return Ok(result);
        }
    }
}