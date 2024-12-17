using CM.Dtos.User;

namespace CM.ApplicationService.UserModule.Abstracts
{
    public interface IUserService
    {
        Task<UserDto> CreateUser(CreateUserDto createUserDto);
        Task<UserDto> UpdateUser(UpdateUserDto updateUserDto);
        Task<bool> DeleteUser(int userId);
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(int id);
    }
}
