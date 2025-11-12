using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<Client>> GetAllClients()
        //{
        //    return await _context.Clients
        //        .Include(c => c.Customer).ThenInclude(u => u.User)
        //        .Include(c => c.Bank)
        //        .ToListAsync();
        //}

        public async Task<PagedResult<Client>> GetAllClients(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {

            var query = _context.Clients
                .Include(c => c.Customer).ThenInclude(cust => cust.User)
                .Include(c => c.Bank)
                .AsNoTracking();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    c.CompanyName.ToLower().Contains(searchTerm) ||
                    c.AccountNumber.ToLower().Contains(searchTerm) ||
                    (c.Bank != null && c.Bank.BankName.ToLower().Contains(searchTerm)) ||
                    (c.Customer != null && c.Customer.User != null && (
                        c.Customer.User.FirstName.ToLower().Contains(searchTerm) ||
                        c.Customer.User.LastName.ToLower().Contains(searchTerm) ||
                        c.Customer.User.UserName.ToLower().Contains(searchTerm)
                    ))
                );
            }


            var totalCount = await query.CountAsync();


            bool isDescending = sortOrder == SortOrder.DESC;

            if (!string.IsNullOrWhiteSpace(sortColumn))
            {

                switch (sortColumn.ToLower())
                {
                    case "accountnumber":
                        query = isDescending
                            ? query.OrderByDescending(c => c.AccountNumber)
                            : query.OrderBy(c => c.AccountNumber);
                        break;
                    case "balance":
                        query = isDescending
                            ? query.OrderByDescending(c => c.Balance)
                            : query.OrderBy(c => c.Balance);
                        break;
                    case "lastname":
                        query = isDescending
                            ? query.OrderByDescending(c => c.Customer.User.LastName)
                            : query.OrderBy(c => c.Customer.User.LastName);
                        break;
                    case "bankname":
                        query = isDescending
                            ? query.OrderByDescending(c => c.Bank.BankName)
                            : query.OrderBy(c => c.Bank.BankName);
                        break;
                    case "companyname":
                    default:
                        query = isDescending
                            ? query.OrderByDescending(c => c.CompanyName)
                            : query.OrderBy(c => c.CompanyName);
                        break;
                }
            }
            else
            {

                query = query.OrderBy(c => c.CompanyName);
            }


            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Client>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<Client?> GetClientById(int id)
        {
            return await _context.Clients
                .Include(c => c.Customer).ThenInclude(u => u.User)
                .Include(c => c.Bank)
                .FirstOrDefaultAsync(c => c.ClientId == id);
        }

        public async Task<Client?> GetClientByCustomerId(int customerId)
        {
            return await _context.Clients
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<Client> AddClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<Client> UpdateClient(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<bool> DeleteClient(int id)
        {
            var existing = await _context.Clients.FindAsync(id);
            if (existing == null) return false;

            _context.Clients.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
