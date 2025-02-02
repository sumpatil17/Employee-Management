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
    public class ProjectsRepository : Repository<Projects>, IProject
    {
        public readonly EmployeeDBContext _db;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));
        public ProjectsRepository(EmployeeDBContext context) : base(context)
        {
            _db = context;
        }

        public async Task<APIProjectsList> GetProjectList(int page, int pageSize, string search)
        {
            try
            {
                APIProjectsList projectListCount = new APIProjectsList();
                List<Projects> projects = await _db.Projects.
                    Select
                    (
                        d => new Projects
                        {
                            Id = d.Id,
                            ProjectCode = d.ProjectCode,
                            ProjectName = d.ProjectName,
                            OrganizationName = d.OrganizationName
                        }
                    ).OrderByDescending(r => r.Id).ToListAsync();


                if (!string.IsNullOrEmpty(search))
                {
                    projects = projects
                   .Where(r => (r.ProjectName ?? string.Empty).ToLower().Contains(search.ToLower()) || (r.ProjectCode ?? string.Empty).ToLower().Contains(search.ToLower()) || (r.OrganizationName ?? string.Empty).ToLower().Contains(search.ToLower()))
                   .ToList();
                }

                projectListCount.Count = projects.Count();

                if (page != -1)
                    projects = projects.Skip((page - 1) * pageSize).ToList();

                if (pageSize != -1)
                    projects = projects.Take(pageSize).ToList();


                projectListCount.List = projects.ToList();

                return projectListCount;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Projects> Post(Projects project)
        {
            try
            {
                _db.Projects.Add(project);
                await _db.SaveChangesAsync();
                return project;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Projects> GetProjectById(int id)
        {
            try
            {
                Projects project = await _db.Projects.Where(d => d.Id == id).FirstOrDefaultAsync();
                return project;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Projects> UpdateProject(Projects project)
        {
            try
            {
                _db.Projects.Update(project);
                await _db.SaveChangesAsync();
                return project;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Projects> DeleteProject(Projects project)
        {
            try
            {
                _db.Projects.Remove(project);
                await _db.SaveChangesAsync();
                return project;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<bool> CheckProjectDependency(int id)
        {
            try
            {
                bool isExist = await _db.Employee.Where(d => d.ProjectId == id).AnyAsync();
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
