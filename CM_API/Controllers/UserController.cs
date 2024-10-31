using CM.Auth.ApplicantService.AuthModule.Implements;
using CM.Auth.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerDto)
        {
            var result = await _userService.Register(registerDto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _userService.Login(loginDto);
            return Ok(new { Token = token });
        }

        [Authorize(Policy = "RequireCreateUserPermission")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser(RegisterUserDto registerDto)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var result = await _userService.CreateUser(registerDto, userId);
            return Ok(result);
        }
    }
}
