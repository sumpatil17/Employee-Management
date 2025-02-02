using Employee_Management.APIModel;
using Employee_Management.Data;
using Employee_Management.Helper;
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
        private readonly IConfiguration _configuration;
        public readonly EmployeeDBContext _db;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));
        private readonly string DefaultPassword;
        public EmployeeRepository(IConfiguration configuration, EmployeeDBContext context)
        { 
            _configuration = configuration;
            _db = context;
            DefaultPassword = _configuration["DefaultPassword"];
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

        public async Task<APIEmployeeList> GetEmployeeList(int page, int pageSize, string search)
        {
            try
            {
                APIEmployeeList employeeListCount = new APIEmployeeList();
                List<APIEmployee> employee = await _db.Employee.
                    Select
                    (
                        d => new APIEmployee
                        {
                            EmployeeId = d.EmployeeId, 
                            EmployeeName = d.EmployeeName,
                            DoJInFormat = d.DateofJoining.HasValue ? d.DateofJoining.Value.ToString("dd MMM yyyy") : null,
                            DepartmentId = d.DepartmentId,
                            PhotoFileName = d.PhotoFileName,
                            DepartmentName = _db.Department.Where(a=>a.DepartmentId == d.DepartmentId).Select(a=>a.DepartmentName).FirstOrDefault(),
                            UserId  = EncryptionHelper.Decrypt(d.UserId),
                            EmailId = EncryptionHelper.Decrypt(d.EmailId),
                            MobileNo = EncryptionHelper.Decrypt(d.MobileNo)
                        
                        }
                    ).OrderByDescending(r => r.EmployeeId).ToListAsync();


                if (!string.IsNullOrEmpty(search))
                {
                    employee = employee
                   .Where(r => (r.EmployeeName ?? string.Empty).ToLower().Contains(search.ToLower()))
                   .ToList();
                }

                employeeListCount.Count = employee.Count();

                if (page != -1)
                    employee = employee.Skip((page - 1) * pageSize).ToList();

                if (pageSize != -1)
                    employee = employee.Take(pageSize).ToList();


                employeeListCount.List = employee.ToList();

                return employeeListCount;
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
                string hashedPassword = PasswordHelper.GenerateHashedPassword(DefaultPassword);
                employee.Password = hashedPassword;
                employee.AccountCreatedDate = DateTime.Now;
                employee.IsPasswordUpdated = false;
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
