using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<User>> GetAllUsers()
        //{
        //    return await _context.Users.AsNoTracking().ToListAsync();
        //}

        public async Task<PagedResult<User>> GetAllUsers(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            var query = _context.Users.AsNoTracking();

            // 1. SEARCHING
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(searchTerm) ||
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    u.EmailId.ToLower().Contains(searchTerm)
                );
            }

            // Get TOTAL COUNT *after* searching, *before* sorting/pagination
            var totalCount = await query.CountAsync();

            // 2. SORTING
            bool isDescending = sortOrder == SortOrder.DESC;

            // Sort logic
            if (!string.IsNullOrWhiteSpace(sortColumn))
            {
                switch (sortColumn.ToLower())
                {
                    case "username":
                        query = isDescending ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName);
                        break;
                    case "firstname":
                        query = isDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName);
                        break;
                    case "lastname":
                        query = isDescending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName);
                        break;
                    case "email":
                        query = isDescending ? query.OrderByDescending(u => u.EmailId) : query.OrderBy(u => u.EmailId);
                        break;
                    case "userrole":
                        query = isDescending ? query.OrderByDescending(u => u.UserRole) : query.OrderBy(u => u.UserRole);
                        break;
                    default:
                        // Default sort column if specified column is invalid
                        query = query.OrderBy(u => u.UserName);
                        break;
                }
            }
            else
            {
                // Default sort if no column is specified
                query = query.OrderBy(u => u.UserName);
            }

            // 3. PAGINATION
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsUser(int id)
        {
            return await _context.Users.AnyAsync(u => u.UserId == id);
        }

        public async Task<User?> GetUserByUserName(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }


        public async Task<User?> GetByUserName(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmailId == email);
        }

        public async Task<IEnumerable<User>> GetUnassignedBankUsersAsync()
        {
            // Fetch all users with role BANKUSER
            return await _context.Users
       .Where(u => u.UserRole == UserRole.BANKUSER && u.Bank == null)
       .ToListAsync();
        }
    }
}
