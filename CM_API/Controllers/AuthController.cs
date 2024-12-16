using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.AuthModule.Abstracts;
using CM.ApplicationService.RoleModule.Abstracts;
using CM.ApplicationService.UserModule.Abstracts;
using CM.Dtos.Auth.Auth;
using CM.Dtos.Role;
using CM.Dtos.User;
using CM.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;


namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly CMDbContext _dbContext;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            CMDbContext dbContext,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            _logger.LogInformation("Start: Registering a new user with email {Email}.", registerDto.Email);

            try
            {
                var user = await _authService.Register(registerDto);
                _logger.LogInformation("Registration successful for user with email {Email}.", registerDto.Email);
                return Ok(new { Message = "Register successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for email {Email}.", registerDto.Email);
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Start: Logging in {Username}.", loginDto.Username);

            try
            {
                var loginResponse = await _authService.Login(loginDto);
                _logger.LogInformation("Login successful for {Username}.", loginDto.Username);
                return Ok(new { Message = loginResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for {Username}.", loginDto.Username);
                return Unauthorized(new { Error = ex.Message });
            }
        }
    }
}