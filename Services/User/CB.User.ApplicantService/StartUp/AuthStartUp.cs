using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.Auth.ApplicantService.AuthModule.Abstracts;
using CM.Auth.ApplicantService.AuthModule.Implements;
using CM.Auth.ApplicantService.Common.Abstracts;
using CM.Auth.ApplicantService.Common.Implements;
using CM.Auth.ApplicantService.RoleModule.Abstracts;
using CM.Auth.ApplicantService.RoleModule.Implements;
using CM.Auth.ApplicantService.UserModule.Abstracts;
using CM.Auth.ApplicantService.UserModule.Implements;
using CM.Auth.Domain;
using CM.Auth.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Share.Constant.Database;

namespace CM.Auth.ApplicationService.StartUp
{
    public static class AuthStartUp
    {
        public static void ConfigureAuth(this WebApplicationBuilder builder, string? assemblyName)
        {
            builder.Services.AddDbContext<AuthDbContext>(
                options =>
                {
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("Default"),
                        options =>
                        {
                            options.MigrationsAssembly(assemblyName);
                            options.MigrationsHistoryTable(
                                DbSchema.TableMigrationsHistory,
                                DbSchema.User
                            );
                        }
                    );
                },
                ServiceLifetime.Scoped
            );
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IValidateEmailService, ValidateEmailService>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            // Cấu hình JWT Authentication

            builder
                .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])
                        ),
                    };
                });

        }
    }
}
