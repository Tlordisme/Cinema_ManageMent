using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.Common;
using CM.Infrastructure;
using Microsoft.Extensions.Logging;



namespace CM.Auth.ApplicantService.Permission.Implements
{
    public class PermissionService : ServiceBase, IPermissionService
    {

        public PermissionService(CMDbContext dbContext, ILogger<PermissionService> logger)
            : base(logger, dbContext)
        {
        }

        public List<string> GetPermissions(int userId)
        {
            _logger.LogInformation("Fetching permissions for user with ID {UserId}", userId);
            var roleIds = _dbContext
                .UserRoles.Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .Distinct().ToList();
            if (!roleIds.Any())
            {
                _logger.LogWarning("User with ID {UserId} has no roles assigned", userId);
                return new List<string>();
            }
            var permissions = _dbContext
                .RolePermissions.Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionKey)
                .Distinct()
                .ToList();
            _logger.LogInformation(
                "Permissions fetched for user ID {UserId}: {Permissions}",
                userId,
                permissions
            );
            return permissions;
        }

        public bool CheckPermission(int userId, string permissionKey)
        {
            _logger.LogInformation("Checking if user with ID {UserId} has permission {PermissionKey}", userId, permissionKey);

            var userPermissions = GetPermissions(userId);

            // Kiểm tra xem quyền yêu cầu có nằm trong danh sách quyền của người dùng không
            var hasPermission = userPermissions.Contains(permissionKey, StringComparer.OrdinalIgnoreCase);

            if (hasPermission)
            {
                _logger.LogInformation("User with ID {UserId} has permission {PermissionKey}", userId, permissionKey);
            }
            else
            {
                _logger.LogWarning("User with ID {UserId} does NOT have permission {PermissionKey}", userId, permissionKey);
            }

            return hasPermission;
        }
    }
}