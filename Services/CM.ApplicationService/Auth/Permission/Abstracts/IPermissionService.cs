using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicantService.Auth.Permission.Abstracts
{
    public interface IPermissionService
    {
        public List<string> GetPermissions(int userId);
        public bool CheckPermission(int userId, string permissionKey);
    }
}