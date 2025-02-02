using Employee_Management.Model;
using System.Collections.Generic;

namespace Employee_Management.APIModel
{
    public class APIProjects
    {
        public string ProjectCode { get; set; }
        public string OrganizationName { get; set; }
        public string ProjectName { get; set; }
    }

    public class APIProjectsList
    {
        public List<Projects> List { get; set; }
        public int Count { get; set; }
    }


}
