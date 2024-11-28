using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.UserModule.Abstracts;
using CM.Dtos.User;
using CM.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CM_API.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;

        private readonly CMDbContext _dbContext;

        public UserController(IUserService userService, IPermissionService permissionService, CMDbContext dbContext)
        {
            _userService = userService;
            _permissionService = permissionService;

            _dbContext = dbContext;
        }

        [HttpPost("Createusers")]
        [Authorize]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);

            // Kiểm tra quyền "CreateUser"
            if (!_permissionService.CheckPermission(userId, "CreateUser"))
            {
                return Unauthorized("You do not have permission to create users.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdUser = await _userService.CreateUser(createUserDto);
            return CreatedAtAction(nameof(GetUserData), new { id = createdUser.Id }, createdUser);
        }

        // Lấy thông tin người dùng theo ID
        [HttpGet("Getusers/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserData(int id)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value); // Lấy userId từ JWT claim

            // Kiểm tra quyền "ViewUser"
            if (!_permissionService.CheckPermission(userId, "ViewUser"))
            {
                return Unauthorized("You do not have permission to view users.");
            }

            var users = await _userService.GetAllUsers();
            var foundUser = users.FirstOrDefault(u => u.Id == id);

            if (foundUser == null)
            {
                return NotFound();
            }

            return Ok(foundUser);
        }

        [HttpGet("GetAllusers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value); // Lấy userId từ JWT claim

            // Kiểm tra quyền "ViewAllUsers"
            if (!_permissionService.CheckPermission(userId, "ViewAllUsers"))
            {
                return Unauthorized("You do not have permission to view all users.");
            }

            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpPut("Updateusers")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value); // Lấy userId từ JWT claim

            // Kiểm tra quyền "UpdateUser"
            if (!_permissionService.CheckPermission(userId, "UpdateUser"))
            {
                return Unauthorized("You do not have permission to update users.");
            }

            var updatedUser = await _userService.UpdateUser(updateUserDto);
            return Ok(updatedUser);
        }

        [HttpDelete("Deleteusers/{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value); // Lấy userId từ JWT claim

            // Kiểm tra quyền "DeleteUser"
            if (!_permissionService.CheckPermission(currentUserId, "DeleteUser"))
            {
                return Unauthorized("You do not have permission to delete users.");
            }

            var result = await _userService.DeleteUser(userId);
            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpGet("activate")]
        public async Task<IActionResult> ActivateAccount(string email, string token)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.IsActive)
                return BadRequest("Người dùng không tồn tại hoặc đã được kích hoạt.");

            // Kiểm tra token
            var rawToken = $"{user.Email}:{user.DateOfBirth:yyyyMMddHHmmss}";
            var expectedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawToken));
            if (token != expectedToken)
                return Unauthorized("Token không hợp lệ.");

            // Kích hoạt tài khoản
            user.IsActive = true;
            await _dbContext.SaveChangesAsync();

            return Ok("Tài khoản đã được kích hoạt thành công.");
        }
    }
}