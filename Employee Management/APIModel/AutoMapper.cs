using AutoMapper;
using Employee_Management.Model;

namespace Employee_Management.APIModel
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Department, APIDepartment>()
                  .ReverseMap();
            CreateMap<Employee, APIEmployee>()
                  .ReverseMap();
            CreateMap<Roles, APIRoles>()
                  .ReverseMap();
            CreateMap<Projects, APIProjects>()
                  .ReverseMap();
        }
    }
}
