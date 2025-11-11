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


            var totalCount = await query.CountAsync();


            bool isDescending = sortOrder == SortOrder.DESC;


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

                        query = query.OrderBy(u => u.UserName);
                        break;
                }
            }
            else
            {

                query = query.OrderBy(u => u.UserName);
            }


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
            return await _context.Users
        .Include(u => u.Customer)
        .FirstOrDefaultAsync(u => u.UserId == id);
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

        public async Task<IEnumerable<User>> GetAvailableClientUsers()
        {
            return await _context.Users
              .AsNoTracking()

                      .Where(u => u.UserRole == UserRole.CLIENTUSER &&
                                  !_context.Customers.Any(c => c.UserId == u.UserId))
              .ToListAsync();
        }
        public async Task<IEnumerable<User>> GetUnassignedBankUsersAsync()
        {

            return await _context.Users
       .Where(u => u.UserRole == UserRole.BANKUSER && u.Bank == null)
       .ToListAsync();
        }
    }
}