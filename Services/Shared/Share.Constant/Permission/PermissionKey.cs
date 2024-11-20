using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Constant.Permission
{
    public static class PermissionKey
    {

        //User
        public const string CreateUser = "create_user";
        public const string UpdateUser = "update_user";
        public const string DeleteUser = "delete_user";
        public const string ViewUser = "view_users";
        public const string ViewAllUser = "view_all_users";

        //Role
        public const string GetAllRoles = "get_all_roles";
        public const string GetRoleById = "get_role_by_id";
        public const string CreateRole = "create_role";
        public const string UpdateRole = "update_role";
        public const string DeleteRole = "delete_role";

        public const string AddRoleToUser = "add_role_to_user";
        public const string GetUserRoles = "get_user_roles";
    }
}