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
using CM.ApplicationService.Notification.Abstracts;
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
        //private readonly IEmailService _emailService;

        public AuthService(
            CMDbContext dbContext,
            ILogger<AuthService> logger,
            IPasswordHasher<User> passwordHasher,
            ValidateService validateService,
            JwtService jwtService
            //IEmailService emailService
        )
            : base(logger, dbContext)
        {
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _validateService = validateService;
            //_emailService = emailService;
        }


        //private string GenerateActivationToken(User user)
        //{
        //    var rawToken = $"{user.Email}:{user.DateOfBirth:yyyyMMddHHmmss}";
        //    return Convert.ToBase64String(Encoding.UTF8.GetBytes(rawToken));
        //}
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
                PhoneNumber = registerDto.PhoneNumber,
                Gender = registerDto.Gender,
                DateOfBirth = registerDto.DateOfBirth,
                Password = _passwordHasher.HashPassword(null, registerDto.Password),
                //IsActive = false
                
            };

            _dbContext.Users.Add(user);
    //        var token = GenerateActivationToken(user);
    //        var activationLink = $"https://yourdomain.com/api/auth/activate?email={user.Email}&token={token}";

    //        var emailBody = $@"
    //        <h1>Chào mừng {user.FullName}!</h1>
    //        <p>Nhấn vào liên kết bên dưới để kích hoạt tài khoản:</p>
    //        <a href='{activationLink}'>Kích hoạt tài khoản</a>
    //        <p>Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email này.</p>
    //";

    //        await _emailService.SendEmailAsync(user.Email, "Kích hoạt tài khoản", emailBody);

            _logger.LogInformation($"User {user.UserName} registered successfully. Activation email sent.");


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


    }
}