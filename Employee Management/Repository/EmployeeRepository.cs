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
    public class EmployeeRepository : IEmployee
    {
        public readonly EmployeeDBContext _db;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));
        public EmployeeRepository(EmployeeDBContext context)
        { 
            _db = context;
        }

        public async Task<List<Employee>> Get()
        {
            try
            {
                List<Employee> employees = await  _db.Employee.
                    Select
                    (
                       e => new Employee
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeName = e.EmployeeName,
                            DateofJoining = e.DateofJoining,
                            DepartmentId =  e.DepartmentId,
                           PhotoFileName = e.PhotoFileName,
                        }
                    ).ToListAsync();
                return employees;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Employee> Post(Employee employee)
        {
            try
            {
                _db.Employee.Add(employee);
                await _db.SaveChangesAsync();
                return employee;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            try
            {
                Employee employee = await _db.Employee.Where(d => d.EmployeeId == id).FirstOrDefaultAsync();
                return employee;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Employee> UpdateEmployee(Employee employee)
        {
            try
            {
                _db.Employee.Update(employee);
                await _db.SaveChangesAsync();
                return employee;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }

        public async Task<Employee> DeleteEmployee(Employee employee)
        {
            try
            {
                _db.Employee.Remove(employee);
                await _db.SaveChangesAsync();
                return employee;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }


    }
}
