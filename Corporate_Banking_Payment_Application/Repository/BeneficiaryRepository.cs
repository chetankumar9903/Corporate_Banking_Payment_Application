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

        //public async Task<IEnumerable<Beneficiary>> GetAllBeneficiaries()
        //{
        //    return await _context.Beneficiaries
        //        .Include(b => b.Client)
        //        .AsNoTracking()
        //        .ToListAsync();
        //}

        public async Task<PagedResult<Beneficiary>> GetAllBeneficiaries(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            // Base query MUST include dependencies for searching and sorting
            var query = _context.Beneficiaries
                .Include(b => b.Client)
                .AsNoTracking();

            // 1. SEARCHING
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(b =>
                    b.BeneficiaryName.ToLower().Contains(searchTerm) ||
                    b.AccountNumber.ToLower().Contains(searchTerm) ||
                    b.BankName.ToLower().Contains(searchTerm) ||
                    (b.IfscCode != null && b.IfscCode.ToLower().Contains(searchTerm)) ||
                    (b.Client != null && b.Client.CompanyName.ToLower().Contains(searchTerm))
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
                    case "accountnumber":
                        query = isDescending ? query.OrderByDescending(b => b.AccountNumber) : query.OrderBy(b => b.AccountNumber);
                        break;
                    case "bankname":
                        query = isDescending ? query.OrderByDescending(b => b.BankName) : query.OrderBy(b => b.BankName);
                        break;
                    case "ifsccode":
                        query = isDescending ? query.OrderByDescending(b => b.IfscCode) : query.OrderBy(b => b.IfscCode);
                        break;
                    case "companyname": // Sort by client's company name
                        query = isDescending ? query.OrderByDescending(b => b.Client.CompanyName) : query.OrderBy(b => b.Client.CompanyName);
                        break;
                    case "beneficiaryname":
                    default:
                        query = isDescending ? query.OrderByDescending(b => b.BeneficiaryName) : query.OrderBy(b => b.BeneficiaryName);
                        break;
                }
            }
            else
            {
                // Default sort
                query = query.OrderBy(b => b.BeneficiaryName);
            }

            // 3. PAGINATION
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Beneficiary>
            {
                Items = items,
                TotalCount = totalCount
            };
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
