using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.Auth.ApplicantService.AuthModule.Abstracts;
using CM.Auth.ApplicantService.RoleModule.Abstracts;
using CM.Auth.ApplicantService.UserModule.Implements;
using CM.Auth.Domain;
using CM.Auth.Dtos.Role;
using CM.Auth.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Share.ApplicationService;

namespace CM.Auth.ApplicantService.RoleModule.Implements
{
    public class RoleService : BaseService, IRoleService
    {
        private readonly AuthDbContext _context;

        public RoleService(AuthDbContext context, ILogger<RoleService> logger)
            : base(logger)
        {
            _context = context;
        }

        public async Task<RoleDto> CreateRole(CreateRoleDto createRoleDto)
        {
            var role = new Role { Name = createRoleDto.Name };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            LogInformation($"Role {role.Name} created successfully.");
            return new RoleDto { Id = role.Id, Name = role.Name };
        }

        public async Task<IEnumerable<RoleDto>> GetAllRoles()
        {
            return await _context
                .Roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name })
                .ToListAsync();
        }

        public async Task<RoleDto> GetRoleById(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                LogWarning($"Role with ID {roleId} not found.");
                throw new KeyNotFoundException("Role not found.");
            }

            return new RoleDto { Id = role.Id, Name = role.Name };
        }

        public async Task<RoleDto> UpdateRole(UpdateRoleDto updateRoleDto)
        {
            var role = await _context.Roles.FindAsync(updateRoleDto.Id);
            if (role == null)
            {
                LogWarning($"Role with ID {updateRoleDto.Id} not found.");
                throw new KeyNotFoundException("Role not found.");
            }

            role.Name = updateRoleDto.Name;

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            LogInformation($"Role {role.Name} updated successfully.");

            return new RoleDto { Id = role.Id, Name = role.Name };
        }

        public async Task<bool> DeleteRole(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                LogWarning($"Role with ID {roleId} not found.");
                throw new KeyNotFoundException("Role not found.");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            LogInformation($"Role {role.Name} deleted successfully.");
            return true;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(User user)
        {

            return await _context
                .UserRoles.Where(ur => ur.UserId == user.Id)
                .Select(ur =>
                    _context
                        .Roles.Where(r => r.Id == ur.RoleId)
                        .Select(r => r.Name)
                        .FirstOrDefault()
                )
                .ToListAsync();
        }
    }
}
