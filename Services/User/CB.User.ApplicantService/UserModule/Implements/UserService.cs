using CM.Auth.ApplicantService.Common.Abstracts;
using CM.Auth.ApplicantService.UserModule.Abstracts;
using CM.Auth.Domain;
using CM.Auth.Dtos;
using CM.Auth.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Share.ApplicationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Auth.ApplicantService.UserModule.Implements
{
    public class UserService : BaseService, IUserService
    {
        private readonly AuthDbContext _context;
        private readonly IValidateEmailService _validateEmailService;
        public UserService(AuthDbContext context, IValidateEmailService validateEmailService, ILogger<UserService> logger): base(logger)
        {
            _context = context;
            _validateEmailService = validateEmailService;
        }
        public async Task<UserDto> CreateUser(CreateUserDto createUserDto)
        {
            if (!_validateEmailService.IsValidEmail(createUserDto.Email))
            {
                LogWarning($"Invalid email format for {createUserDto.Email}.");
                throw new ArgumentException("Email không hợp lệ.");
            }
            var user = new User
            {
                Email = createUserDto.Email,
                UserName = createUserDto.UserName,
                FullName = createUserDto.FullName,
                Password = new PasswordHasher<User>().HashPassword(null, createUserDto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            LogInformation($"User {user.UserName} created successfully.");
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName
            };
        }

        public async Task<UserDto> UpdateUser(UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(updateUserDto.Id);
            if (user == null)
            {
                LogWarning($"User with ID {updateUserDto.Id} not found.");
                throw new KeyNotFoundException("User not found.");
            }

            user.UserName = updateUserDto.UserName;
            user.Email = updateUserDto.Email;
            user.FullName = updateUserDto.FullName;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            LogInformation($"User {user.UserName} updated successfully.");

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName
            };
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                LogWarning($"User with ID {userId} not found.");
                throw new KeyNotFoundException("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            LogInformation($"User {user.UserName} deleted successfully.");
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FullName = u.FullName
                }).ToListAsync();

            LogInformation("Fetched all users successfully.");
            return users;
        }
    }
}
