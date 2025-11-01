using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Repository
{
    public class SalaryDisbursementRepository : ISalaryDisbursementRepository
    {
        private readonly AppDbContext _context;

        public SalaryDisbursementRepository(AppDbContext context)
        {
            _context = context;
        }
        private IQueryable<SalaryDisbursement> BaseQuery()
        {
            return _context.SalaryDisbursements
                .Include(s => s.Client)
                .Include(s => s.Employee)
                .AsQueryable();
        }

        //public async Task<IEnumerable<SalaryDisbursement>> GetAll()
        //{
        //    return await _context.SalaryDisbursements
        //        .Include(s => s.Employee)
        //        .Include(s => s.Client)
        //        .Include(s => s.BatchTransaction)
        //        .AsNoTracking()
        //        .ToListAsync();
        //}

        public async Task<PagedResult<SalaryDisbursement>> GetAll(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            var query = BaseQuery().AsNoTracking();

            // 1. SEARCHING
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(s =>
                    (s.Employee != null && (s.Employee.FirstName.ToLower().Contains(searchTerm) || (s.Employee.LastName != null && s.Employee.LastName.ToLower().Contains(searchTerm)))) ||
                    (s.Client != null && s.Client.CompanyName.ToLower().Contains(searchTerm)) ||
                    (s.Description != null && s.Description.ToLower().Contains(searchTerm)) ||
                    (s.BatchId.HasValue && s.BatchId.ToString().Contains(searchTerm)) // Search by BatchId
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
                    case "companyname":
                        query = isDescending ? query.OrderByDescending(s => s.Client.CompanyName) : query.OrderBy(s => s.Client.CompanyName);
                        break;
                    case "lastname":
                        query = isDescending ? query.OrderByDescending(s => s.Employee.LastName) : query.OrderBy(s => s.Employee.LastName);
                        break;
                    case "amount":
                        query = isDescending ? query.OrderByDescending(s => s.Amount) : query.OrderBy(s => s.Amount);
                        break;
                    case "date":
                        query = isDescending ? query.OrderByDescending(s => s.Date) : query.OrderBy(s => s.Date);
                        break;
                    default:
                        query = isDescending ? query.OrderByDescending(s => s.Date) : query.OrderBy(s => s.Date);
                        break;
                }
            }
            else
            {
                // Default sort
                query = query.OrderByDescending(s => s.Date);
            }

            // 3. PAGINATION
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<SalaryDisbursement>
            {
                Items = items,
                TotalCount = totalCount
            };
        }


        public async Task<SalaryDisbursement?> GetById(int id)
        {
            return await _context.SalaryDisbursements
                .Include(s => s.Employee)
                .Include(s => s.Client)
                .FirstOrDefaultAsync(s => s.SalaryDisbursementId == id);
        }

        public async Task<IEnumerable<SalaryDisbursement>> GetByClientId(int clientId)
        {
            return await _context.SalaryDisbursements
                .Where(s => s.ClientId == clientId)
                .Include(s => s.Employee)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<SalaryDisbursement>> GetByEmployeeId(int employeeId)
        {
            return await _context.SalaryDisbursements
                .Where(s => s.EmployeeId == employeeId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SalaryDisbursement> Add(SalaryDisbursement entity)
        {
            _context.SalaryDisbursements.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }


        public async Task Update(SalaryDisbursement entity)
        {
            _context.SalaryDisbursements.Update(entity);
            await _context.SaveChangesAsync();

        }


        public async Task Delete(SalaryDisbursement entity)
        {
            _context.SalaryDisbursements.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
