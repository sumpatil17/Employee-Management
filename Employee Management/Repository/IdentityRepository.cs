using Employee_Management.Model;
using Employee_Management.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Employee_Management.Data;
using log4net;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management.Repository
{
    public class IdentityRepository : IIdentity
    {
        private readonly IConfiguration _configuration;
        public readonly EmployeeDBContext _db;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));

        public IdentityRepository(IConfiguration configuration, EmployeeDBContext context)
        {
            _configuration = configuration;
            _db = context;
        }

        public async Task<Employee> GetStoredPasswordHash(string username)
        {
            try
            {
                Employee pw = await _db.Employee.Where(e => e.EmployeeName == username).FirstOrDefaultAsync();           
                return pw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return null;
            }
        }
    }
}
