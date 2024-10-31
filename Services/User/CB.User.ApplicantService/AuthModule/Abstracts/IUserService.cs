using CM.Auth.Domain;
using CM.Auth.Dtos;
using Share.Constant.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CM.Auth.ApplicantService.AuthModule.Abstracts
{
    public interface IUserService
    {
        Task<string> Register(RegisterUserDto registerDto);
        Task<string> Login(LoginDto loginDto);
        Task<string> CreateUser(RegisterUserDto createDto, int CreateUserId);

    }
}
