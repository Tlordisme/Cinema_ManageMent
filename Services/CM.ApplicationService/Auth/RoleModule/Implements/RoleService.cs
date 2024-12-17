using CM.ApplicationService.Common;
using CM.ApplicationService.RoleModule.Abstracts;
using CM.Domain.Auth;
using CM.Dtos.Role;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class RoleService : ServiceBase, IRoleService
{
    public RoleService(CMDbContext dbContext, ILogger<RoleService> logger)
        : base(logger, dbContext) { }

    public async Task<RoleDto> CreateRole(CreateRoleDto createRoleDto)
    {
        bool roleExist = await _dbContext.Roles.AnyAsync(r =>
            r.Name.ToLower() == createRoleDto.Name.ToLower()
        );
        if (roleExist)
        {
            _logger.LogWarning($"Role {createRoleDto.Name} already exists.");
            throw new InvalidOperationException("Role already exists.");
        }

        var role = new Role { Name = createRoleDto.Name };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Role {role.Name} created successfully.");
        return new RoleDto { Id = role.Id, Name = role.Name };
    }

    public async Task<IEnumerable<RoleDto>> GetAllRoles()
    {
        return await _dbContext
            .Roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name })
            .ToListAsync();
    }

    public async Task<RoleDto> GetRoleById(int roleId)
    {
        var role = await _dbContext.Roles.FindAsync(roleId);
        if (role == null)
        {
            _logger.LogWarning($"Role with ID {roleId} not found.");
            throw new KeyNotFoundException("Role not found.");
        }

        return new RoleDto { Id = role.Id, Name = role.Name };
    }

    public async Task<RoleDto> UpdateRole(UpdateRoleDto updateRoleDto)
    {
        var role = await _dbContext.Roles.FindAsync(updateRoleDto.Id);
        if (role == null)
        {
            _logger.LogWarning($"Role with ID {updateRoleDto.Id} not found.");
            throw new KeyNotFoundException("Role not found.");
        }
        var existingRole = await _dbContext
            .Roles.Where(r => r.Name.ToLower() == updateRoleDto.Name.ToLower())
            .FirstOrDefaultAsync();

        if (existingRole != null)
        {
            _logger.LogWarning($"Role with name {updateRoleDto.Name} already exists.");
            throw new InvalidOperationException("Role with this name already exists.");
        }

        role.Name = updateRoleDto.Name;

        _dbContext.Roles.Update(role);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Role {role.Name} updated successfully.");

        return new RoleDto { Id = role.Id, Name = role.Name };
    }

    public async Task<bool> DeleteRole(int roleId)
    {
        var role = await _dbContext.Roles.FindAsync(roleId);
        if (role == null)
        {
            _logger.LogWarning($"Role with ID {roleId} not found.");
            throw new KeyNotFoundException("Role not found.");
        }

        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Role {role.Name} deleted successfully.");
        return true;
    }

    public async Task<bool> AddRoleToUser(int userId, string roleName)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"User with ID {userId} not found.");
            throw new KeyNotFoundException("User not found.");
        }

        var role = await _dbContext.Roles.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == roleName.ToLower()
        );

        if (role == null)
        {
            _logger.LogWarning($"Role {roleName} not found.");
            throw new KeyNotFoundException("Role not found.");
        }

        var existingUserRole = await _dbContext.UserRoles.FirstOrDefaultAsync(ur =>
            ur.UserId == userId && ur.RoleId == role.Id
        );

        if (existingUserRole != null)
        {
            _logger.LogInformation($"User {userId} already has the role {roleName}.");
            return false;
        }

        var userRole = new UserRole { UserId = userId, RoleId = role.Id };

        _dbContext.UserRoles.Add(userRole);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Role {roleName} assigned to user {userId} successfully.");
        return true;
    }

    public async Task<IEnumerable<string?>> GetUserRoles(User user)
    {
        return await _dbContext
            .UserRoles.Where(ur => ur.UserId == user.Id)
            .Select(ur =>
                _dbContext
                    .Roles.Where(r => r.Id == ur.RoleId)
                    .Select(r => r.Name)
                    .FirstOrDefault()
            )
            .ToListAsync();
    }
}
