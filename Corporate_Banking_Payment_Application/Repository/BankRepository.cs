using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Repository
{
    public class BankRepository : IBankRepository
    {
        private readonly AppDbContext _context;

        public BankRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bank>> GetAllBank()
        {
            return await _context.Banks
                .Include(b => b.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Bank?> GetBankById(int id)
        {
            return await _context.Banks
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BankId == id);
        }

        public async Task<Bank> AddBank(Bank bank)
        {
            _context.Banks.Add(bank);
            await _context.SaveChangesAsync();
            return bank;
        }

        public async Task UpdateBank(Bank bank)
        {
            _context.Banks.Update(bank);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBank(Bank bank)
        {
            _context.Banks.Remove(bank);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsBank(int id)
        {
            return await _context.Banks.AnyAsync(b => b.BankId == id);
        }
    }
}
