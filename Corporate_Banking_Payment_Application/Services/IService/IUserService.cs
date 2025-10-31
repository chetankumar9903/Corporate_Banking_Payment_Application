using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<UserDto?> GetUserById(int id);
        Task<UserDto> CreateUser(CreateUserDto dto);
        Task<UserDto?> UpdateUser(int id, UpdateUserDto dto);
        Task<bool> DeleteUser(int id);

        Task<IEnumerable<User>> GetUnassignedBankUsersAsync();
    }
}
