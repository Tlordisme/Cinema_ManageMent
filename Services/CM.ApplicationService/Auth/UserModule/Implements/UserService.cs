using CM.ApplicationService.Auth.Common;
using CM.ApplicationService.Common;
using CM.ApplicationService.UserModule.Abstracts;
using CM.Domain.Auth;
using CM.Dtos.User;
using CM.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class UserService : ServiceBase, IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly CMDbContext _dbContext;
    private readonly ValidateService _validateService;

    public UserService(CMDbContext dbContext, ILogger<UserService> logger, ValidateService validateService)
        : base(logger, dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        _validateService = validateService;
    }

    // Tạo người dùng mới
    public async Task<UserDto> CreateUser(CreateUserDto createUserDto)
    {
        await _validateService.ValidateEmailAsync(createUserDto.Email);
        await _validateService.ValidateUserNameAsync(createUserDto.UserName);
        _validateService.ValidateFullName(createUserDto.FullName);
        _validateService.ValidateDateOfBirth(createUserDto.DateOfBirth);

        var user = new User
        {
            Email = createUserDto.Email,
            UserName = createUserDto.UserName,
            FullName = createUserDto.FullName,
            PhoneNumber = createUserDto.PhoneNumber,
            Password = new PasswordHasher<User>().HashPassword(null, createUserDto.Password),
            DateOfBirth = createUserDto.DateOfBirth,
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Ghi log sau khi người dùng được tạo thành công
        _logger.LogInformation($"User {user.UserName} created successfully.");

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FullName = user.FullName
        };
    }

    // Cập nhật người dùng
    public async Task<UserDto> UpdateUser(UpdateUserDto updateUserDto)
    {
        var user = await _dbContext.Users.FindAsync(updateUserDto.Id);
        if (user == null)
        {
            _logger.LogWarning($"User with ID {updateUserDto.Id} not found.");
            throw new KeyNotFoundException("User not found.");
        }

        await _validateService.ValidateEmailAsync(updateUserDto.Email);
        await _validateService.ValidateUserNameAsync(updateUserDto.UserName);
        _validateService.ValidateFullName(updateUserDto.FullName);
        _validateService.ValidateDateOfBirth(updateUserDto.DateOfBirth);

        user.UserName = updateUserDto.UserName;
        user.Email = updateUserDto.Email;
        user.FullName = updateUserDto.FullName;
        user.PhoneNumber = updateUserDto.PhoneNumber;
        user.Gender = updateUserDto.Gender;
        user.DateOfBirth = updateUserDto.DateOfBirth;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"User {user.UserName} updated successfully.");

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

    // Xóa người dùng
    public async Task<bool> DeleteUser(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"User with ID {userId} not found.");
            throw new KeyNotFoundException("User not found.");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"User {user.UserName} deleted successfully.");
        return true;
    }

    // Lấy danh sách tất cả người dùng
    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        var users = await _dbContext
            .Users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FullName = u.FullName,
                Gender = u.Gender
            })
            .ToListAsync();

        _logger.LogInformation("Fetched all users successfully.");
        return users;
    }

    public async Task<UserDto> GetUserById(int id)
    {
        var user = await _dbContext.Users
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FullName = u.FullName,
                Gender = u.Gender
            })
            .FirstOrDefaultAsync();

        _logger.LogInformation("Fetched user successfully.");
        return user;
    }
}
