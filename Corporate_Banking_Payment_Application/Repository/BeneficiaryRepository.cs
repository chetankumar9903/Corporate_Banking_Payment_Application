using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
namespace Corporate_Banking_Payment_Application.Repository
{
    public class BeneficiaryRepository : IBeneficiaryRepository
    {
        private readonly AppDbContext _context;

        public BeneficiaryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Beneficiary>> GetAllBeneficiaries()
        {
            return await _context.Beneficiaries
                .Include(b => b.Client)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Beneficiary?> GetBeneficiaryById(int id)
        {
            return await _context.Beneficiaries
                .Include(b => b.Client)
                .FirstOrDefaultAsync(b => b.BeneficiaryId == id);
        }

        public async Task<IEnumerable<Beneficiary>> GetBeneficiariesByClientId(int clientId)
        {
            return await _context.Beneficiaries
                .Where(b => b.ClientId == clientId)
                .Include(b => b.Client)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Beneficiary> AddBeneficiary(Beneficiary beneficiary)
        {
            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync();
            return beneficiary;
        }

        public async Task<Beneficiary> UpdateBeneficiary(Beneficiary beneficiary)
        {
            _context.Beneficiaries.Update(beneficiary);
            await _context.SaveChangesAsync();
            return beneficiary;
        }

        public async Task<bool> DeleteBeneficiary(int id)
        {
            var existing = await _context.Beneficiaries.FindAsync(id);
            if (existing == null) return false;

            _context.Beneficiaries.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
