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
using System.Collections;
using Employee_Management.Repository;
using log4net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.IO;

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
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));
        
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
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
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
                return Ok(new
                {
                    Code = "DEPARTMENT_ADDED",
                    Message = "Department added successfully.",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("UpdateDepartment/{id:int}")]
        public async Task<IActionResult> Update(int id,[FromBody] APIDepartment apiDepartment)
        {
            try
            {
                Department department = await _department.GetDepartmentById(id);
                if(department == null)
                {
                    _logger.Error($"Attempt to delete non-existent department with ID {id}");
                    return NotFound(new
                    {
                        code = "DEPARTMENT_NOT_FOUND",
                        message = $"Department with ID {id} does not exist.",
                        statusCode = 404
                    });
                }
                department.DepartmentName = apiDepartment.DepartmentName;
                department = await _department.UpdateDepartment(department);
                return Ok(new
                {
                    Code = "DEPARTMENT_UPDATED",
                    Message = "Department updated successfully.",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                Department department = await _department.GetDepartmentById(id);
                if (department == null)
                {
                    _logger.Error($"Attempt to delete non-existent department with ID {id}");
                    return NotFound(new
                    {
                        code = "DEPARTMENT_NOT_FOUND",
                        message = $"Department with ID {id} does not exist.",
                        statusCode = 404
                    });
                }
                bool isDependencyExist = await _department.CheckDepartmentDependency(id);
                if (isDependencyExist)
                {
                    _logger.Error($"Attempt to delete department with ID {id} failed due to employee dependencies.");
                    return Conflict(new
                    {
                        code = "DEPARTMENT_DEPENDENCY",
                        message = $"The department with ID {id} cannot be deleted because employees are associated with it.",
                        statusCode = 409
                    });
                }
                department = await _department.DeleteDepartment(department);
                return Ok(new
                {
                    Code = "DEPARTMENT_DELETED",
                    Message = "Department deleted successfully.",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }

        

    }
}
