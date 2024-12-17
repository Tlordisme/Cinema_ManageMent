using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.UserModule.Abstracts;
using CM.Domain.Auth;
using CM.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IPermissionService _permissionService;

    public UserController(IUserService userService, IPermissionService permissionService)
    {
        _userService = userService;
        _permissionService = permissionService;
    }

    [HttpPost("CreateUser")]
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
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    [HttpGet("GetUser/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(int id)
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value);

        // Kiểm tra quyền "ViewUser"
        if (!_permissionService.CheckPermission(userId, "ViewUser"))
        {
            return Unauthorized("You do not have permission to view this user.");
        }

        var user = await _userService.GetUserById(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user); 
    }

    [HttpPut("UpdateUsers/{userId}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value);

        // Kiểm tra quyền "UpdateUser"
        if (!_permissionService.CheckPermission(userId, "UpdateUser"))
        {
            return Unauthorized("You do not have permission to update users.");
        }

        var updatedUser = await _userService.UpdateUser(updateUserDto);
        return Ok(updatedUser);
    }

    [HttpDelete("DeleteUsers/{userId}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

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

    [HttpGet("GetAllUsers")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value);

        // Kiểm tra quyền "ViewUsers"
        if (!_permissionService.CheckPermission(userId, "ViewUsers"))
        {
            return Unauthorized("You do not have permission to view users.");
        }

        var users = await _userService.GetAllUsers();
        return Ok(users);
    }
}
