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

            var query = _context.Customers
                .Include(c => c.User)
                .Include(c => c.Bank)
                .AsNoTracking();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c =>

                    (c.User != null && (
                        c.User.UserName.ToLower().Contains(searchTerm) ||
                        c.User.FirstName.ToLower().Contains(searchTerm) ||
                        c.User.LastName.ToLower().Contains(searchTerm)
                    )) ||

                    (c.Bank != null &&
                        c.Bank.BankName.ToLower().Contains(searchTerm)
                    )
                );
            }


            var totalCount = await query.CountAsync();


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
                query = query.OrderByDescending(c => c.OnboardingDate);
            }


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
