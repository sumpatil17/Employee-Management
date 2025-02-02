using Employee_Management.APIModel;
using Employee_Management.Data;
using Employee_Management.Model;
using Employee_Management.Repository.Interface;
using log4net;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management.Repository
{
    public class RolesRepository : Repository<Roles>, IRoles
    {
        public readonly EmployeeDBContext _db;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));
        public RolesRepository(EmployeeDBContext context) : base(context)
        {
            _db = context;
        }

        public async Task<APIRolesList> GetRolesList(int page, int pageSize, string search)
        {
            try
            {
                APIRolesList rolesListCount = new APIRolesList();
                List<Roles> roles = await _db.Roles.
                    Select
                    (
                        d => new Roles
                        {
                            Id = d.Id,
                            RoleCode = d.RoleCode,
                            RoleName = d.RoleName
                        }
                    ).OrderByDescending(r => r.Id).ToListAsync();


                if (!string.IsNullOrEmpty(search))
                {
                    roles = roles
                   .Where(r => (r.RoleName ?? string.Empty).ToLower().Contains(search.ToLower()) || (r.RoleCode ?? string.Empty).ToLower().Contains(search.ToLower()))
                   .ToList();
                }

                rolesListCount.Count = roles.Count();

                if (page != -1)
                    roles = roles.Skip((page - 1) * pageSize).ToList();

                if (pageSize != -1)
                    roles = roles.Take(pageSize).ToList();


                rolesListCount.List = roles.ToList();

                return rolesListCount;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Roles> Post(Roles role)
        {
            try
            {
                _db.Roles.Add(role);
                await _db.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Roles> GetRoleById(int id)
        {
            try
            {
                Roles role = await _db.Roles.Where(d => d.Id == id).FirstOrDefaultAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Roles> UpdateRole(Roles role)
        {
            try
            {
                _db.Roles.Update(role);
                await _db.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Roles> DeleteRole(Roles role)
        {
            try
            {
                _db.Roles.Remove(role);
                await _db.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<bool> CheckRoleDependency(int id)
        {
            try
            {
                bool isExist = await _db.Employee.Where(d => d.RoleId == id).AnyAsync();
                return isExist;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return true;
            }
        }
    }    
}
