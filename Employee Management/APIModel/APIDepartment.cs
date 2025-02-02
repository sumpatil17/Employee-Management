using Employee_Management.Model;
using System.Collections.Generic;

namespace Employee_Management.APIModel
{
    public class APIDepartment
    {
        public string DepartmentName { get; set; }
    }

    public class APIDepartmentList
    {
        public List<Department> List { get; set; }
        public int Count { get; set; }
    }


}
