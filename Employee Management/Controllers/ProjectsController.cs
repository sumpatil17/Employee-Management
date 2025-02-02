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
  //  [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IProject _project;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));

        public ProjectsController(IConfiguration configuration, IMapper mapper,IProject project)
        {
            _configuration = configuration;
            _mapper = mapper;
            _project = project;
        }


        [HttpGet("GetProjects/{page:int?}/{pageSize:int?}/{search?}")]
        public async Task<IActionResult> GetProjects(int page, int pageSize, string search)
        {
            try
            {
                APIProjectsList projects = await _project.GetProjectList(page, pageSize, search);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddProject")]
        public async Task<IActionResult> Post([FromBody] APIProjects aPIProjects)
        {
            try
            {
                Projects projects = _mapper.Map<Projects>(aPIProjects);
                Projects response = await _project.Post(projects);
                return Ok(new
                {
                    Code = "PROJECT_ADDED",
                    Message = "Project added successfully.",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("UpdateProject/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] APIProjects apiProject)
        {
            try
            {
                Projects project = await _project.GetProjectById(id);
                if (project == null)
                {
                    _logger.Error($"Attempt to update non-existent project with ID {id}");
                    return NotFound(new
                    {
                        code = "PROJECT_NOT_FOUND",
                        message = $"Project with ID {id} does not exist.",
                        statusCode = 404
                    });
                }
                project.ProjectCode = apiProject.ProjectCode;
                project.OrganizationName = apiProject.OrganizationName;
                project.ProjectName = apiProject.ProjectName;
                project = await _project.UpdateProject(project);
                return Ok(new
                {
                    Code = "PROJECT_UPDATED",
                    Message = "Project updated successfully.",
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
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                Projects projects = await _project.GetProjectById(id);
                if (projects == null)
                {
                    _logger.Error($"Attempt to delete non-existent project with ID {id}");
                    return NotFound(new
                    {
                        code = "PROJECT_NOT_FOUND",
                        message = $"Project with ID {id} does not exist.",
                        statusCode = 404
                    });
                }
                bool isDependencyExist = await _project.CheckProjectDependency(id);
                if (isDependencyExist)
                {
                    _logger.Error($"Attempt to delete project with ID {id} failed due to employee dependencies.");
                    return BadRequest(new
                    {
                        code = "PROJECT_DEPENDENCY",
                        message = $"The project with name {projects.ProjectName} cannot be deleted because employees are associated with it.",
                        statusCode = 409
                    });
                }
                projects = await _project.DeleteProject(projects);
                return Ok(new
                {
                    Code = "PROJECT_DELETED",
                    Message = "Project deleted successfully.",
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
