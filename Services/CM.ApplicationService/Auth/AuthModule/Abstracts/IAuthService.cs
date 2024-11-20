using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CM.Dtos.Auth.Auth;
using CM.Dtos.User;
using Microsoft.AspNetCore.Identity.Data;
using Share.Constant.Permission;

namespace CM.ApplicationService.AuthModule.Abstracts
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginDto loginDto);
        Task<UserDto> Register(RegisterUserDto registerDto);
    }
}
