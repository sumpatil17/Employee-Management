using Employee_Management.APIModel;
using Employee_Management.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Employee_Management.Repository.Interface
{
    public interface IRoles
    {
        Task<APIRolesList> GetRolesList(int page, int pageSize, string search);
        Task<Roles> Post(Roles role);
        Task<Roles> GetRoleById(int id);
        Task<Roles> UpdateRole(Roles role);
        Task<bool> CheckRoleDependency(int id);
        Task<Roles> DeleteRole(Roles role);
    }
}
