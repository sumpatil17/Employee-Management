using Employee_Management.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace Employee_Management.Data
{
    public class EmployeeDBContext : DbContext  
    {
        public EmployeeDBContext(DbContextOptions<EmployeeDBContext> options) : base(options) { }

        public DbSet<Department> Department { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Projects> Projects { get; set; }
    }
}
