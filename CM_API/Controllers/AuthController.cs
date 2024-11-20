using CM.ApplicationService.AuthModule.Abstracts;
using CM.ApplicationService.RoleModule.Abstracts;
using CM.ApplicationService.UserModule.Abstracts;
using CM.Dtos.Auth.Auth;
using CM.Dtos.Role;
using CM.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]


    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _authService.Register(registerDto);
            return Ok(new { Message = "Register successful" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResponse = await _authService.Login(loginDto);
            return Ok(new { Message = loginResponse });
        }
    }
}