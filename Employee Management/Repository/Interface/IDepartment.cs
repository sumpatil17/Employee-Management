using Employee_Management.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Employee_Management.Repository.Interface
{
    public interface IDepartment
    {
        Task<List<Department>> Get();
        Task<Department> Post(Department department);
        Task<Department> GetDepartmentById(int id);
        Task<Department> UpdateDepartment(Department department);
        Task<Department> DeleteDepartment(Department department);
        Task<bool> CheckDepartmentDependency(int id);
    }
}
