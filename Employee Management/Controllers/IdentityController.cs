using Employee_Management.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Employee_Management.Repository.Interface;
using Employee_Management.Helper;
using AutoMapper;
using Employee_Management.Repository;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Employee_Management.APIModel;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Employee_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class IdentityController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IIdentity _identity;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DepartmentRepository));

        public IdentityController(IConfiguration configuration, IIdentity identity)
        {
            _configuration = configuration;
            _identity = identity;
        }

        [HttpPost("VerifyPassword")]
        public async Task<IActionResult> VerifyPassword([FromBody] Credentials credentials)
        {
            try
            {
                string decryptedPayload = EncryptionHelper.Decrypt(credentials.Credential);
                var creds = JsonConvert.DeserializeObject<Cred>(decryptedPayload);
                creds.UserId = EncryptionHelper.Encrypt(creds.UserId);

                Employee employee =await _identity.GetStoredPasswordHash(creds.UserId);
                bool isPasswordCorrect = PasswordHelper.VerifyPassword(creds.Password, employee.Password);
                if(isPasswordCorrect)
                {
                    // Generate JWT token
                    string token = JwtTokenHelper.GenerateToken(employee.EmployeeName, employee.EmployeeId, _configuration);
                    var response = new { AccessToken = token, IsPasswordUpdated = employee.IsPasswordUpdated, IsSuccessful = true };
                    string encryptedResponse = EncryptionHelper.Encrypt(JsonConvert.SerializeObject(response));
                    
                    return Ok(new { encryptedData = encryptedResponse });

                }
                else
                {
                    return Unauthorized(new { message = "Invalid credentials", IsSuccessful = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

   
