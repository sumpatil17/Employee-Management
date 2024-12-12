using Employee_Management.APIModel;
using Employee_Management.Data;
using Employee_Management.Model;
using Employee_Management.Repository.Interface;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public class DepartmentRepository : Repository<Department>,IDepartment
    {
        public readonly EmployeeDBContext _db;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));
        public DepartmentRepository(EmployeeDBContext context) : base(context)
        { 
            _db = context;
        }

        public async Task<List<Department>> Get()
        {
            try
            {
                List<Department> department = await  _db.Department.
                    Select
                    (
                        d => new Department
                        {
                           DepartmentId =  d.DepartmentId,
                           DepartmentName =  d.DepartmentName
                        }
                    ).ToListAsync();
                return department;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Department> Post(Department department)
        {
            try
            {
                _db.Department.Add(department);
                await _db.SaveChangesAsync();
                return department;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Department> GetDepartmentById(int id)
        {
            try
            {
                Department department = await _db.Department.Where(d=>d.DepartmentId == id).FirstOrDefaultAsync();
                return department;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Department> UpdateDepartment(Department department)
        {
            try
            {
                _db.Department.Update(department);
                await _db.SaveChangesAsync();
                return department;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Department> DeleteDepartment(Department department)
        {
            try
            {
                _db.Department.Remove(department);
                await _db.SaveChangesAsync();
                return department;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<bool> CheckDepartmentDependency(int id)
        {
            try
            {
                bool isExist = await _db.Employee.Where(d => d.DepartmentId == id).AnyAsync();
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
