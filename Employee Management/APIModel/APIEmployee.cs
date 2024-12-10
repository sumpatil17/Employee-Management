using System;

namespace Employee_Management.APIModel
{
    public class APIEmployee
    {
        public string EmployeeName { get; set; }
        public DateTime? DateofJoining { get; set; }
        public int DepartmentId { get; set; }
        public string PhotoFileName { get; set; }
    }
}
