using Employee_Management.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Employee_Management.Repository.Interface
{
    public interface IIdentity
    {
        Task<Employee> GetStoredPasswordHash(string userId);
    }
}
