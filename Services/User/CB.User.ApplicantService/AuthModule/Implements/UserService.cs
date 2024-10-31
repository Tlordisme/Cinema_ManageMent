using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CM.Auth.ApplicantService.AuthModule.Abstracts;
using CM.Auth.Domain;
using CM.Auth.Dtos;
using CM.Auth.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Share.ApplicationService;
using Share.Constant.Permission;

namespace CM.Auth.ApplicantService.AuthModule.Implements
{
    public class UserService : BaseService, IUserService
    {
        private readonly AuthDbContext _context;
        private readonly IJwtService _jwtService;

        public UserService(
            AuthDbContext context,
            IJwtService jwtService,
            ILogger<UserService> logger
        )
            : base(logger)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<string> Register(RegisterUserDto registerDto)
        {
            if (_context.Users.Any(u => u.UserName == registerDto.UserName))
                throw new Exception("Username already exists.");

            // Hash password và lưu user
            var user = new User
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),

                FullName = registerDto.FullName,
                Gender = registerDto.Gender,
                DateOfBirth = registerDto.DateOfBirth,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return "User registered successfully.";
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u =>
                u.UserName == loginDto.Username
            );
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                throw new Exception("Invalid credentials.");

            // Lấy các role và permission
            var roles = await (
                from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id
                where ur.UserId == user.Id
                select r.Name
            ).ToListAsync();

            var permissions = await (
                from ur in _context.UserRoles
                join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
                where ur.UserId == user.Id
                select rp.PermissionKey
            ).ToListAsync();

            // Tạo JWT token
            return _jwtService.GenerateToken(user, roles, permissions);
        }

        public async Task<string> CreateUser(RegisterUserDto registerDto, int creatorUserId)
        {
            // Check quyền tạo user
            var hasPermission = await (
                from ur in _context.UserRoles
                join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
                where ur.UserId == creatorUserId && rp.PermissionKey == "CreateUser"
                select rp
            ).AnyAsync();
            if (!hasPermission)
                throw new Exception("Not authorized to create user.");

            return await Register(registerDto);
        }
    }
}
