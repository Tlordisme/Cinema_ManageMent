using CM.Auth.ApplicantService.AuthModule.Abstracts;
using CM.Auth.ApplicantService.RoleModule.Abstracts;
using CM.Auth.ApplicantService.UserModule.Abstracts;
using CM.Auth.Dtos;
using CM.Auth.Dtos.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    
    public class AuthController : Controller
    {

        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AuthController(IAuthService authService, IUserService userService, IRoleService roleService)
        {
            _authService = authService;
            _userService = userService;
            _roleService = roleService;
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var user = await _authService.Register(registerDto);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResponse = await _authService.Login(loginDto);
            return Ok(loginResponse);
        }

        [HttpPost("Createusers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var createdUser = await _userService.CreateUser(createUserDto);
            return CreatedAtAction(nameof(GetUserData), new { id = createdUser.Id }, createdUser);
        }

        // Lấy thông tin người dùng theo ID (chỉ dành cho Admin)
        [HttpGet("Getusers/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserData(int id)
        {
            var users = await _userService.GetAllUsers();
            var foundUser = users.FirstOrDefault(u => u.Id == id);  // So sánh int với int

            if (foundUser == null)
            {
                return NotFound();
            }

            return Ok(foundUser);
        }


        [HttpGet("GetAllusers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpPut("Updateusers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateUser(updateUserDto);
            return Ok(updatedUser);
        }

        [HttpDelete("Deleteusers/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _userService.DeleteUser(userId);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }


        [HttpPost("Createroles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            var createdRole = await _roleService.CreateRole(createRoleDto);
            return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, createdRole);
        }

        // Lấy tất cả vai trò
        [HttpGet("GetAllroles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRoles();
            return Ok(roles);
        }

        // Lấy vai trò theo ID
        [HttpGet("Getroles/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleById(id);
            return Ok(role);
        }

        // Cập nhật vai trò
        [HttpPut("Updateroles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            var updatedRole = await _roleService.UpdateRole(updateRoleDto);
            return Ok(updatedRole);
        }

        // Xóa vai trò
        [HttpDelete("DeleteRoles/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var result = await _roleService.DeleteRole(roleId);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
