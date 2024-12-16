using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.UserModule.Abstracts;
using CM.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, IPermissionService permissionService, ILogger<UserController> logger)
    {
        _userService = userService;
        _permissionService = permissionService;
        _logger = logger;  
    }

    [HttpPost("CreateUser")]
    [Authorize]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value);

        _logger.LogInformation("User with ID {UserId} is attempting to create a new user", userId); 

        // Kiểm tra quyền "CreateUser"
        if (!_permissionService.CheckPermission(userId, "CreateUser"))
        {
            _logger.LogWarning("User with ID {UserId} does not have permission to create users", userId); 
            return Unauthorized("You do not have permission to create users.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid when attempting to create user with ID {UserId}", userId); 
            return BadRequest(ModelState);
        }

        var createdUser = await _userService.CreateUser(createUserDto);
        _logger.LogInformation("User with ID {UserId} successfully created a new user with ID {CreatedUserId}", userId, createdUser.Id); 
        return CreatedAtAction(nameof(GetUserData), new { id = createdUser.Id }, createdUser);
    }

    [HttpGet("GetUser/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserData(int id)
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value); 

        _logger.LogInformation("User with ID {UserId} is attempting to get user data for user with ID {UserIdToFetch}", userId, id);

        // Kiểm tra quyền "ViewUser"
        if (!_permissionService.CheckPermission(userId, "ViewUser"))
        {
            _logger.LogWarning("User with ID {UserId} does not have permission to view user with ID {UserIdToFetch}", userId, id); 
            return Unauthorized("You do not have permission to view users.");
        }

        var users = await _userService.GetAllUsers();
        var foundUser = users.FirstOrDefault(u => u.Id == id);

        if (foundUser == null)
        {
            _logger.LogWarning("User with ID {UserId} tried to fetch user with ID {UserIdToFetch}, but user was not found", userId, id); 
            return NotFound();
        }

        return Ok(foundUser);
    }

    [HttpPut("UpdateUsers/{userId}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value);

        _logger.LogInformation("User with ID {UserId} is attempting to update user with ID {UpdateUserId}", userId, updateUserDto.Id);

        // Kiểm tra quyền "UpdateUser"
        if (!_permissionService.CheckPermission(userId, "UpdateUser"))
        {
            _logger.LogWarning("User with ID {UserId} does not have permission to update user with ID {UpdateUserId}", userId, updateUserDto.Id);
            return Unauthorized("You do not have permission to update users.");
        }

        var updatedUser = await _userService.UpdateUser(updateUserDto);
        _logger.LogInformation("User with ID {UserId} successfully updated user with ID {UpdatedUserId}", userId, updatedUser.Id); 
        return Ok(updatedUser);
    }

    [HttpDelete("DeleteUsers/{userId}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value); 

        _logger.LogInformation("User with ID {CurrentUserId} is attempting to delete user with ID {UserIdToDelete}", currentUserId, userId);

        // Kiểm tra quyền "DeleteUser"
        if (!_permissionService.CheckPermission(currentUserId, "DeleteUser"))
        {
            _logger.LogWarning("User with ID {CurrentUserId} does not have permission to delete user with ID {UserIdToDelete}", currentUserId, userId); 
            return Unauthorized("You do not have permission to delete users.");
        }

        var result = await _userService.DeleteUser(userId);
        if (result)
        {
            _logger.LogInformation("User with ID {CurrentUserId} successfully deleted user with ID {UserIdToDelete}", currentUserId, userId); 
            return NoContent();
        }

        _logger.LogWarning("User with ID {CurrentUserId} tried to delete user with ID {UserIdToDelete}, but user was not found", currentUserId, userId); 
        return NotFound();
    }
}