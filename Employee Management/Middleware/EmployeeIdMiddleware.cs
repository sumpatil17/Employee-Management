using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Management.Middleware
{
    public class EmployeeIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public EmployeeIdMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length);
                var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

                var employeeIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;

                if (!string.IsNullOrEmpty(employeeIdClaim) && int.TryParse(employeeIdClaim, out var employeeId))
                {
                    context.Items["EmployeeId"] = employeeId;
                }
            }

            await _next(context);
        }
    }
}
