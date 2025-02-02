using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Employee_Management.Data;
using AutoMapper;
using Employee_Management.Repository;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Employee_Management.APIModel;
using System.Threading.Tasks;
using System;
using Employee_Management.Repository.Interface;
using Employee_Management.Model;

namespace Employee_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IRoles _roles;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));

        public RolesController(IConfiguration configuration, IRoles roles, IMapper mapper)
        {
            _configuration = configuration;
            _roles = roles;
            _mapper = mapper;
        }


        [HttpGet("GetRoles/{page:int?}/{pageSize:int?}/{search?}")]
        public async Task<IActionResult> GetRoles(int page, int pageSize, string search)
        {
            try
            {
                APIRolesList roles = await _roles.GetRolesList(page, pageSize, search);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddRoles")]
        public async Task<IActionResult> Post([FromBody] APIRoles aPIRoles)
        {
            try
            {
                Roles roles = _mapper.Map<Roles>(aPIRoles);
                Roles response = await _roles.Post(roles);
                return Ok(new
                {
                    Code = "ROLES_ADDED",
                    Message = "Role added successfully.",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("UpdateRole/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] APIRoles apiRole)
        {
            try
            {
                Roles role = await _roles.GetRoleById(id);
                if (role == null)
                {
                    _logger.Error($"Attempt to update non-existent role with ID {id}");
                    return NotFound(new
                    {
                        code = "ROLE_NOT_FOUND",
                        message = $"Role with ID {id} does not exist.",
                        statusCode = 404
                    });
                }
                role.RoleCode = apiRole.RoleCode;
                role.RoleName = apiRole.RoleName;
                role = await _roles.UpdateRole(role);
                return Ok(new
                {
                    Code = "ROLE_UPDATED",
                    Message = "Role updated successfully.",
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
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                Roles role = await _roles.GetRoleById(id);
                if (role == null)
                {
                    _logger.Error($"Attempt to delete non-existent role with ID {id}");
                    return NotFound(new
                    {
                        code = "ROLE_NOT_FOUND",
                        message = $"Role with ID {id} does not exist.",
                        statusCode = 404
                    });
                }
                bool isDependencyExist = await _roles.CheckRoleDependency(id);
                if (isDependencyExist)
                {
                    _logger.Error($"Attempt to delete role with ID {id} failed due to employee dependencies.");
                    return BadRequest(new
                    {
                        code = "ROLE_DEPENDENCY",
                        message = $"The role with name {role.RoleName} cannot be deleted because employees are associated with it.",
                        statusCode = 409
                    });
                }
                role = await _roles.DeleteRole(role);
                return Ok(new
                {
                    Code = "ROLE_DELETED",
                    Message = "Role deleted successfully.",
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
