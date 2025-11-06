using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Repository
{
    public class BatchTransactionRepository : IBatchTransactionRepository
    {
        private readonly AppDbContext _context;

        public BatchTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        private IQueryable<BatchTransaction> BaseQuery()
        {
            return _context.BatchTransactions
                .Include(b => b.Client)
                .Include(b => b.SalaryDisbursements)
                .ThenInclude(s => s.Employee)
                .AsQueryable();
        }

        public async Task<IEnumerable<BatchTransaction>> GetAll()
        {
            return await _context.BatchTransactions
                .Include(b => b.Client)
                .Include(b => b.SalaryDisbursements)
                //.ThenInclude(s => s.Employee)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<BatchTransaction?> GetById(int id)
        {
            return await _context.BatchTransactions
                .Include(b => b.Client)
                .Include(b => b.SalaryDisbursements)
                .ThenInclude(sd => sd.Employee)
                .FirstOrDefaultAsync(b => b.BatchId == id);
        }

        public async Task<IEnumerable<BatchTransaction>> GetByClientId(int clientId)
        {
            return await _context.BatchTransactions
               .Where(b => b.ClientId == clientId)
               .Include(b => b.SalaryDisbursements)
               .AsNoTracking()
               .ToListAsync();
        }


        public async Task<BatchTransaction> Add(BatchTransaction entity)
        {
            _context.BatchTransactions.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Update(BatchTransaction batch)
        {
            _context.BatchTransactions.Update(batch);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(BatchTransaction entity)
        {
            _context.BatchTransactions.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<BatchTransaction> CreateNewBatch(int clientId, int count, decimal total)
        {
            var batch = new BatchTransaction
            {
                ClientId = clientId,
                TotalTransactions = count,
                TotalAmount = total
            };
            _context.BatchTransactions.Add(batch);
            await _context.SaveChangesAsync();
            return batch;
        }

    }
}
