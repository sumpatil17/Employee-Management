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

namespace Employee_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly EmployeeDBContext _db;
        private readonly IEmployee _employee;
        private readonly IMapper _mapper;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));
        public EmployeeController(IConfiguration configuration, EmployeeDBContext context,IEmployee employee, IMapper mapper)
        {
            _configuration = configuration;
            _db = context;
            _employee = employee;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Employee> employee = await _employee.Get();
                return new JsonResult(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> Post([FromBody] APIEmployee apiEmployee)
        {
            try
            {
                Employee employee = _mapper.Map<Employee>(apiEmployee);
                Employee response = await _employee.Post(employee);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("UpdateEmployee/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] APIEmployee apiEmployee)
        {
            try
            {
                Employee employee = await _employee.GetEmployeeById(id);
                if (employee == null)
                {
                    _logger.Error($"Employee with ID {id} not found.");
                    return NotFound(new
                    {
                        code = "EMPLOYEE_NOT_FOUND",
                        message = $"Employee with ID {id} does not exist.",
                        statusCode = 404
                    });
                }
                employee.EmployeeName = apiEmployee.EmployeeName;
                employee.DateofJoining = apiEmployee.DateofJoining;
                employee.DepartmentId = apiEmployee.DepartmentId;
                employee.PhotoFileName = apiEmployee.PhotoFileName;
                employee = await _employee.UpdateEmployee(employee);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Employee employee = await _employee.GetEmployeeById(id);
                if (employee == null)
                {
                    _logger.Error($"Employee with ID {id} not found.");
                    return NotFound(new
                    {
                        code = "EMPLOYEE_NOT_FOUND",
                        message = $"Employee with ID {id} does not exist.",
                        statusCode = 404
                    });
                }
                employee = await _employee.DeleteEmployee(employee);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
