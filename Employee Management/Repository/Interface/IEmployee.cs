﻿using Employee_Management.APIModel;
using Employee_Management.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Employee_Management.Repository.Interface
{
    public interface IEmployee
    {
        Task<List<Employee>> Get();
        Task<Employee> Post(Employee employee);
        Task<Employee> GetEmployeeById(int id);
        Task<Employee> UpdateEmployee(Employee employee);
        Task<Employee> DeleteEmployee(Employee employee);
        Task<APIEmployeeList> GetEmployeeList(int page, int pageSize, string search);
    }
}
