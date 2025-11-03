using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<Customer>> GetAllCustomers()
        //{
        //    return await _context.Customers
        //        .Include(c => c.User)
        //        .Include(c => c.Bank)
        //        .ToListAsync();
        //}

        public async Task<PagedResult<Customer>> GetAllCustomers(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            // Base query MUST include User and Bank for searching and sorting
            var query = _context.Customers
                .Include(c => c.User)
                .Include(c => c.Bank)
                .AsNoTracking();

            // 1. SEARCHING
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    // Check User properties (if User is not null)
                    (c.User != null && (
                        c.User.UserName.ToLower().Contains(searchTerm) ||
                        c.User.FirstName.ToLower().Contains(searchTerm) ||
                        c.User.LastName.ToLower().Contains(searchTerm)
                    )) ||
                    // Check Bank properties (if Bank is not null)
                    (c.Bank != null &&
                        c.Bank.BankName.ToLower().Contains(searchTerm)
                    )
                );
            }

            // Get TOTAL COUNT *after* searching
            var totalCount = await query.CountAsync();

            // 2. SORTING
            bool isDescending = sortOrder == SortOrder.DESC;

            if (!string.IsNullOrWhiteSpace(sortColumn))
            {
                switch (sortColumn.ToLower())
                {
                    case "lastname":
                        query = isDescending
                            ? query.OrderByDescending(c => c.User.LastName)
                            : query.OrderBy(c => c.User.LastName);
                        break;
                    case "bankname":
                        query = isDescending
                            ? query.OrderByDescending(c => c.Bank.BankName)
                            : query.OrderBy(c => c.Bank.BankName);
                        break;
                    case "verificationstatus":
                        query = isDescending
                            ? query.OrderByDescending(c => c.VerificationStatus)
                            : query.OrderBy(c => c.VerificationStatus);
                        break;
                    case "onboardingdate":
                    default:
                        query = isDescending
                            ? query.OrderByDescending(c => c.OnboardingDate)
                            : query.OrderBy(c => c.OnboardingDate);
                        break;
                }
            }
            else
            {
                // Default sort: Newest customers first
                query = query.OrderByDescending(c => c.OnboardingDate);
            }

            // 3. PAGINATION
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Customer>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<Customer?> GetCustomerById(int id)
        {
            return await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Bank)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task<Customer> AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> DeleteCustomer(int id)
        {
            var existing = await _context.Customers.FindAsync(id);
            if (existing == null) return false;

            _context.Customers.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Customer?> GetCustomerByUserId(int userId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
