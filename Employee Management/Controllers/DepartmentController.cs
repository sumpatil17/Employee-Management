using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using Employee_Management.Data;
using Employee_Management.Repository.Interface;
using System.Collections.Generic;
using Employee_Management.Model;
using System.Threading.Tasks;
using Employee_Management.APIModel;
using AutoMapper;

namespace Employee_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly EmployeeDBContext _db;
        private readonly IDepartment _department;
        private readonly IMapper _mapper;
        public DepartmentController(IConfiguration configuration, EmployeeDBContext context,IDepartment department, IMapper mapper)
        {
            _configuration = configuration;
            _db = context;
            _department = department;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Department> departments = await _department.Get();
                return new JsonResult(departments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddDepartment")]
        public async Task<IActionResult> Post([FromBody] APIDepartment apiDepartment)
        {
            try
            {
                Department department = _mapper.Map<Department>(apiDepartment);
                Department response = await _department.Post(department);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
