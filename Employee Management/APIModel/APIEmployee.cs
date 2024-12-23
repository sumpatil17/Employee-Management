using Employee_Management.Model;
using System;
using System.Collections.Generic;

namespace Employee_Management.APIModel
{
    public class APIEmployee
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? DateofJoining { get; set; }
        public int DepartmentId { get; set; }
        public string PhotoFileName { get; set; }
        public string DepartmentName { get; set; }
    }


    public class APIEmployeeList
    {
        public List<APIEmployee> List { get; set; }
        public int Count { get; set; }
    }
}
