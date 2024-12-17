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

        public AuthController(IAuthService authService, IUserService userService, IPermissionService permissionService, CMDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var user = await _authService.Register(registerDto);
            return Ok(new { Message = "Register successful" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var loginResponse = await _authService.Login(loginDto);
                return Ok(new { Message = loginResponse });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}