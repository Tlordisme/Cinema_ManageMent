
using CM.Domain.Auth;
using CM.Dtos.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.RoleModule.Abstracts
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRoles();
        Task<RoleDto> GetRoleById(int id);
        Task<RoleDto> CreateRole(CreateRoleDto createRoleDto);
        Task<RoleDto> UpdateRole(UpdateRoleDto updateRoleDto);
        Task<bool> DeleteRole(int id);

        Task<bool> AddRoleToUser(int userId, string roleName);
        Task<IEnumerable<string>> GetUserRoles(User user);
    }
}