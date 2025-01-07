using System;

namespace Employee_Management.Model
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? DateofJoining { get; set; }
        public int? DepartmentId { get; set; }
        public string PhotoFileName { get; set; }
        public string Password { get; set; }
        public bool IsPasswordUpdated { get; set; }
        public DateTime AccountCreatedDate { get; set; }
    }
}
