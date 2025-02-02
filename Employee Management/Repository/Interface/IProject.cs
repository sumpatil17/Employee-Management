using Employee_Management.APIModel;
using Employee_Management.Model;
using System.Threading.Tasks;

namespace Employee_Management.Repository.Interface
{
    public interface IProject
    {
        Task<APIProjectsList> GetProjectList(int page, int pageSize, string search);
        Task<Projects> Post(Projects project);
        Task<Projects> GetProjectById(int id);
        Task<Projects> UpdateProject(Projects project);
        Task<bool> CheckProjectDependency(int id);
        Task<Projects> DeleteProject(Projects project);
    }
}
