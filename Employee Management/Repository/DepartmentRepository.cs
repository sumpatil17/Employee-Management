using Employee_Management.APIModel;
using Employee_Management.Data;
using Employee_Management.Model;
using Employee_Management.Repository.Interface;
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
                return null;
            }
        }
    }
}
