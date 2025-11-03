using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface IUserRepository
    {
        //Task<IEnumerable<User>> GetAllUsers();

        Task<PagedResult<User>> GetAllUsers(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<User?> GetUserById(int id);
        Task<User> AddUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(User user);
        Task<bool> ExistsUser(int id);
        Task<User?> GetUserByUserName(string username);

        Task<User?> GetByUserName(string username);
        Task<User?> GetByEmail(string email);

        Task<IEnumerable<User>> GetUnassignedBankUsersAsync();
    }
}
