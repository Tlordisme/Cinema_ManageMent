using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CM.ApplicationService.Auth.Common;
using CM.ApplicationService.AuthModule.Abstracts;
using CM.ApplicationService.Common;
using CM.ApplicationService.RoleModule.Abstracts;
using CM.Auth.ApplicantService.Auth.Abstracts;
using CM.Auth.ApplicantService.Auth.Implements;
using CM.Domain.Auth;
using CM.Dtos.Auth.Auth;
using CM.Dtos.User;
using CM.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Share.Constant.Permission;

namespace CM.ApplicationService.AuthModule.Implements
{
    public class AuthService : ServiceBase, IAuthService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtService _jwtService;
        private readonly ValidateService _validateService;

        public AuthService(
            CMDbContext dbContext,
            ILogger<AuthService> logger,
            IPasswordHasher<User> passwordHasher,
            ValidateService validateService,
            JwtService jwtService
        )
            : base(logger, dbContext)
        {
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _validateService = validateService;
        }

        public async Task<UserDto> Register(RegisterUserDto registerDto)
        {
            await _validateService.ValidateEmailAsync(registerDto.Email);

            await _validateService.ValidateUserNameAsync(registerDto.UserName);

            _validateService.ValidateFullName(registerDto.FullName);

            _validateService.ValidateDateOfBirth(registerDto.DateOfBirth);

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

            //Add role mặc định
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Standard User");
            if (role == null)
            {
                // Tạo vai trò "Standard User" nếu chưa tồn tại
                role = new Role { Name = "Standard User" };
                _dbContext.Roles.Add(role);
                await _dbContext.SaveChangesAsync();
            }
            var userRole = new UserRole { UserId = user.Id, RoleId = role.Id };
            _dbContext.UserRoles.Add(userRole);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"User {user.UserName} created successfully with role 'Standard User'.");

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
            };
        }

        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.UserName == loginDto.Username
            );

            if (user == null)
            {
                _logger.LogWarning($"Invalid login attempt for user {loginDto.Username}.");
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            // Kiểm tra mật khẩu
            if (
                _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password)
                == PasswordVerificationResult.Failed
            )
            {
                _logger.LogWarning($"Invalid login attempt for user {loginDto.Username}.");
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            // Nếu đăng nhập thành công, cập nhật thời gian đăng nhập
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"User {user.UserName} logged in successfully.");

            return new LoginResponseDto
            {
                Token = await _jwtService.GenerateJwtToken(user),
                //UserName = user.UserName,
                //UserId = user.Id.ToString(), // Nếu ID là kiểu Guid, bạn có thể thay đổi tùy ý
                //FullName = user.FullName,
            };
        }

        //private async Task<string> GenerateJwtTokenAsync(User user)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim("Id", user.Id.ToString()), // Add user ID claim
        //        new Claim("Username", user.UserName), // Add username claim
        //        new Claim("Email", user.Email),
        //    };
        //    if (_roleService == null)
        //    {
        //        throw new InvalidOperationException("Role service has not been initialized.");
        //    }

        //    var roles = await _roleService.GetUserRolesAsync(user);
        //    LogInformation($"Roles for user {user.UserName}: {string.Join(", ", roles)}");

        //    var roleClaims = roles.Select(roleName => new Claim(ClaimTypes.Role, roleName));

        //    claims.AddRange(roleClaims);

        //    var key = new SymmetricSecurityKey(
        //        Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])
        //    );
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:Issuer"],
        //        audience: _configuration["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(
        //            Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])
        //        ),
        //        signingCredentials: creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }
}