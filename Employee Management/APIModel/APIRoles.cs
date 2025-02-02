using Employee_Management.Model;
using System.Collections.Generic;

namespace Employee_Management.APIModel
{
    public class APIRoles
    {
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
    }

    public class APIRolesList
    {
        public List<Roles> List { get; set; }
        public int Count { get; set; }
    }


}
