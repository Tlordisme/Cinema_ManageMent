using CM.Auth.Dtos;
using CM.Auth.Dtos.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Auth.ApplicantService.UserModule.Abstracts
{
    public interface IUserService
    {
        Task<UserDto> CreateUser(CreateUserDto createUserDto);
        Task<UserDto> UpdateUser(UpdateUserDto updateUserDto);
        Task<bool> DeleteUser(int userId);
        Task<IEnumerable<UserDto>> GetAllUsers();
    }
}
