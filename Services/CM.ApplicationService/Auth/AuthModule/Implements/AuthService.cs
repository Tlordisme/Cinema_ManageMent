using CM.ApplicationService.Auth.Common;
using CM.ApplicationService.AuthModule.Abstracts;
using CM.ApplicationService.Common;
using CM.Auth.ApplicantService.Auth.Implements;
using CM.Domain.Auth;
using CM.Dtos.Auth.Auth;
using CM.Dtos.User;
using CM.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        _logger.LogInformation("Start: Registering a new user with email {Email}.", registerDto.Email);

        try
        {
            // Validate user 
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
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            //
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Standard User");
            if (role == null)
            {
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during registration for email {Email}.", registerDto.Email);
            throw new Exception("Registration failed", ex);
        }
    }

    public async Task<LoginResponseDto> Login(LoginDto loginDto)
    {
        _logger.LogInformation("Start: Logging in {Username}.", loginDto.Username);

        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
            if (user == null)
            {
                _logger.LogWarning($"Invalid login attempt for user {loginDto.Username}.");
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            // Verify password
            if (_passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password) == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning($"Invalid login attempt for user {loginDto.Username}.");
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            _logger.LogInformation($"User {user.UserName} logged in successfully.");

            return new LoginResponseDto
            {
                Token = await _jwtService.GenerateJwtToken(user),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for {Username}.", loginDto.Username);
            throw new UnauthorizedAccessException("Login failed", ex);
        }
    }
}