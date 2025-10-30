using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        private IQueryable<Payment> GetBaseQuery()
        {
            // Base query includes navigation properties for all read operations
            return _context.Payments
                .Include(p => p.Client)
                .Include(p => p.Beneficiary);
        }

        public async Task<IEnumerable<Payment>> GetAllPayments()
        {
            return await GetBaseQuery()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentById(int id)
        {
            return await GetBaseQuery()
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }

        public async Task<Payment> AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task UpdatePayment(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePayment(Payment payment)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsPayment(int id)
        {
            return await _context.Payments.AnyAsync(p => p.PaymentId == id);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByClientId(int clientId)
        {
            return await GetBaseQuery()
                .Where(p => p.ClientId == clientId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByBeneficiaryId(int beneficiaryId)
        {
            return await GetBaseQuery()
                .Where(p => p.BeneficiaryId == beneficiaryId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatus(Status status)
        {
            return await GetBaseQuery()
                .Where(p => p.PaymentStatus == status)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
