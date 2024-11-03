using CM.Auth.Domain;
using CM.Auth.Dtos.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Auth.ApplicantService.RoleModule.Abstracts
{
    public interface  IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRoles();
        Task<RoleDto> GetRoleById(int id);
        Task<RoleDto> CreateRole(CreateRoleDto createRoleDto);
        Task<RoleDto> UpdateRole(UpdateRoleDto updateRoleDto);
        Task<bool> DeleteRole(int id);
        Task<IEnumerable<string>> GetUserRolesAsync(User user);
    }
}
