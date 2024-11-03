using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CM.Auth.ApplicantService.AuthModule.Abstracts;
using CM.Auth.ApplicantService.RoleModule.Abstracts;
using CM.Auth.Domain;
using CM.Auth.Dtos;
using CM.Auth.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Share.ApplicationService;
using Share.Constant.Permission;

namespace CM.Auth.ApplicantService.AuthModule.Implements
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly AuthDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRoleService _roleService;
        public AuthService(
            AuthDbContext dbContext,
            IConfiguration configuration,
            IPasswordHasher<User> passwordHasher,
            IRoleService roleService,
            ILogger<AuthService> logger
        )
            : base(logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _roleService = roleService;
        }

        public async Task<UserDto> Register(RegisterUserDto registerDto)
        {
            if (
                await _dbContext.Users.AnyAsync(u =>
                    u.Email == registerDto.Email || u.UserName == registerDto.UserName
                )
            )
            {
                LogError($"Exist {registerDto.UserName} or {registerDto.Email}.");
                throw new InvalidOperationException("Email hoặc tên người dùng đã tồn tại.");
            }
            var user = new User
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                FullName = registerDto.FullName,
                Gender = registerDto.Gender,
                DateOfBirth = registerDto.DateOfBirth,
                Password = _passwordHasher.HashPassword(null, registerDto.Password),
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
            };
        }

        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.UserName == loginDto.Username
            );

            if (user == null)
            {
                LogWarning($"Invalid login attempt for user {loginDto.Username}.");
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            // Kiểm tra mật khẩu
            if (_passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password) == PasswordVerificationResult.Failed)
            {
                LogWarning($"Invalid login attempt for user {loginDto.Username}.");
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            // Nếu đăng nhập thành công, cập nhật thời gian đăng nhập
            await _dbContext.SaveChangesAsync();

            LogInformation($"User {user.UserName} logged in successfully.");

            return new LoginResponseDto
            {
                Token = await GenerateJwtTokenAsync(user),
                UserName = user.UserName,
                UserId = user.Id.ToString(), // Nếu ID là kiểu Guid, bạn có thể thay đổi tùy ý
                FullName = user.FullName
            };
        }


        private async Task<string> GenerateJwtTokenAsync(User user)
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            };
            if (_roleService == null)
            {
                throw new InvalidOperationException("Role service has not been initialized.");
            }

            var roles = await _roleService.GetUserRolesAsync(user);
            LogInformation($"Roles for user {user.UserName}: {string.Join(", ", roles)}");
            var roleClaims = roles.Select(roleName => new Claim(ClaimTypes.Role, roleName));

            // Thay vì claims.AddRange(roleClaims), ta dùng claims.AddRange(roleClaims.ToList())
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
