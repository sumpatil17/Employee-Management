using System.Collections.Generic;
using System.Threading.Tasks;

namespace Employee_Management.Repository.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task Add(TEntity entity);
        Task AddRange(IEnumerable<TEntity> entities);

        Task Update(TEntity entity);
    }
}
