using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CM.ApplicationService.Common;
using CM.Auth.ApplicantService.Auth.Abstracts;
using CM.Auth.ApplicantService.Permission.Implements;
using CM.Domain.Auth;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CM.Auth.ApplicantService.Auth.Implements
{
    public class JwtService : ServiceBase, IJwtService
    {

        public readonly IConfiguration _configuration;

        public readonly PermissionService _permissionService;

        public JwtService(
            IConfiguration configuration,
            CMDbContext dbContext,
            ILogger<JwtService> logger,
            PermissionService permissionService
        )
            : base(logger, dbContext)
        {
            _configuration = configuration;
            _permissionService = permissionService;
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            _logger.LogInformation("Generating token for user ID {UserId}", user.Id);
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()), // Add user ID claim
                new Claim("Username", user.UserName), // Add username claim
                new Claim("Email", user.Email),
            };


            // Add each permission as a claim
            //var permissions = _permissionService.GetPermissions(user.Id);
            //foreach (var permission in permissions)
            //{
            //    claims.Add(new Claim("Permission", permission));
            //}

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])
                ),
                signingCredentials: creds
            );
            _logger.LogInformation("Token generated successfully for user ID {UserId}", user.Id);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}