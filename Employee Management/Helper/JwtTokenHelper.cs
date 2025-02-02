using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;

namespace Employee_Management.Helper
{
    public static class JwtTokenHelper
    {
        public static string GenerateToken(string employeeName, int employeeId, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("EmployeeName", employeeName),
                new Claim("EmployeeId", employeeId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static int? GetEmployeeIdFromToken(string token, IConfiguration configuration)
        {
            try
            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = key,
                    ValidateLifetime = false // Don't validate expiration here
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var employeeIdClaim = principal.FindFirst("EmployeeId");

                if (employeeIdClaim != null && int.TryParse(employeeIdClaim.Value, out int employeeId))
                {
                    return employeeId;
                }

                return null;
            }
            catch
            {
                return null; // Token is invalid or EmployeeId is not present
            }
        }

    }
}
