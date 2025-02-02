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
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Configuration;
using Employee_Management.Helper;
using Newtonsoft.Json;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;

namespace Employee_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IEmployee _employee;
        private readonly IMapper _mapper;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));
        private readonly string _uploadFolder;

        public EmployeeController(IConfiguration configuration,IEmployee employee, IMapper mapper)
        {
            _configuration = configuration;
            _employee = employee;
            _mapper = mapper;
            _uploadFolder = _configuration["ImageUploadFolder"];
        }
       
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Employee> employee = await _employee.Get();
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetEmployeeList/{page:int?}/{pageSize:int?}/{search?}")]
        public async Task<IActionResult> GetEmployeeList(int page, int pageSize, string search)
        {
            try
            {
                APIEmployeeList employee = await _employee.GetEmployeeList(page, pageSize, search);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> Post([FromBody] APIEmployee apiEmployee)
        {
            try
            {
                Employee employee = _mapper.Map<Employee>(apiEmployee);

                employee.UserId = EncryptionHelper.Encrypt(employee.UserId);
                employee.EmailId = EncryptionHelper.Encrypt(employee.EmailId);
                employee.MobileNo = EncryptionHelper.Encrypt(employee.MobileNo);

                Employee response = await _employee.Post(employee);
                return Ok(new
                {
                    Code = "EMPLOYEE_ADDED",
                    Message = "Employee added successfully.",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
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
                employee.UserId = EncryptionHelper.Encrypt(apiEmployee.UserId); ;
                employee.EmailId = EncryptionHelper.Encrypt(apiEmployee.EmailId); ;
                employee.MobileNo = EncryptionHelper.Encrypt(apiEmployee.MobileNo); ;
                employee.DateofJoining = apiEmployee.DateofJoining;
                employee.DepartmentId = apiEmployee.DepartmentId;
                employee.PhotoFileName = apiEmployee.PhotoFileName;
                employee = await _employee.UpdateEmployee(employee);
                return Ok(new
                {
                    Code = "EMPLOYEE_UPDATED",
                    Message = "Employee updated successfully.",
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
                return Ok(new
                {
                    Code = "EMPLOYEE_DELETED",
                    Message = "Employee deleted successfully.",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
               
                if (file == null || file.Length == 0)
                    return BadRequest(new
                    {
                        Code = "NO_FILE",
                        Message = "No file uploaded",
                        StatusCode = 400
                    });

                string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);

                if (!Directory.Exists(_uploadFolder))
                {
                    Directory.CreateDirectory(_uploadFolder);
                }

                string filePath = Path.Combine(_uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string relativePath = Path.Combine(fileName);
                return Ok(new { FilePath = relativePath });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] Credentials credentials)
        {
            try
            {

                if (!(HttpContext.Items["EmployeeId"] is int employeeId))
                {
                    return Unauthorized(new { message = "EmployeeId not found in the token" });
                }

                string decryptedPayload = EncryptionHelper.Decrypt(credentials.Credential);
                var passwordChangeRequest = JsonConvert.DeserializeObject<PasswordChangeRequest>(decryptedPayload);

                Employee employee = await _employee.GetEmployeeById((int)employeeId);

                if (employee == null)
                {
                    return NotFound(new { message = "User not found", IsSuccessful = false });
                }

                bool isOldPasswordCorrect = PasswordHelper.VerifyPassword(passwordChangeRequest.OldPassword, employee.Password);
                if (!isOldPasswordCorrect)
                {
                    return BadRequest(new { message = "Old password is not correct", IsSuccessful = false });
                }

                string newHashedPassword = PasswordHelper.GenerateHashedPassword(passwordChangeRequest.NewPassword);


                employee.Password = newHashedPassword;
                employee.IsPasswordUpdated = true;
                employee = await _employee.UpdateEmployee(employee);
                return Ok(new
                {
                    Code = "PASSWORD_UPDATED",
                    Message = "Password updated successfully.",
                    StatusCode = 200
                });

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetLoggedInUserDetails")]
        public async Task<IActionResult> GetLoggedInUserDetails()
        {
            try
            {

                if (!(HttpContext.Items["EmployeeId"] is int employeeId))
                {
                    return Unauthorized(new { message = "EmployeeId not found in the token" });
                }

                Employee employee = await _employee.GetEmployeeById(employeeId);

                if (employee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                return Ok(new
                {
                    EmployeeName =  employee.EmployeeName,
                    PhotoFileName = employee.PhotoFileName         
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
