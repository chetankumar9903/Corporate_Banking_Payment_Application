using Corporate_Banking_Payment_Application.DTOs;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<UserDto?> GetUserById(int id);
        Task<UserDto> CreateUser(CreateUserDto dto);
        Task<UserDto?> UpdateUser(int id, UpdateUserDto dto);
        Task<bool> DeleteUser(int id);
    }
}
