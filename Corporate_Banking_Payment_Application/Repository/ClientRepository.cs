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

        public async Task<IEnumerable<Client>> GetAllClients()
        {
            return await _context.Clients
                .Include(c => c.Customer).ThenInclude(u => u.User)
                .Include(c => c.Bank)
                .ToListAsync();
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
