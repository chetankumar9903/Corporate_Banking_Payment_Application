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

        public async Task<IEnumerable<SalaryDisbursement>> GetAll()
        {
            return await _context.SalaryDisbursements
                .Include(s => s.Employee)
                .Include(s => s.Client)
                .Include(s => s.BatchTransaction)
                .AsNoTracking()
                .ToListAsync();
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
