using CM.Auth.Domain;
using CM.Auth.Dtos;
using Microsoft.AspNetCore.Identity.Data;
using Share.Constant.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CM.Auth.ApplicantService.AuthModule.Abstracts
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginDto loginDto);
        Task<UserDto> Register(RegisterUserDto registerDto);
    }
}
