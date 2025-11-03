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

        //public async Task<IEnumerable<Payment>> GetAllPayments()
        //{
        //    return await GetBaseQuery()
        //        .AsNoTracking()
        //        .ToListAsync();
        //}

        public async Task<PagedResult<Payment>> GetAllPayments(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            var query = GetBaseQuery().AsNoTracking();

            // 1. SEARCHING
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(p =>
                    // Client User details
                    (p.Client != null && p.Client.Customer != null && p.Client.Customer.User != null && (
                        p.Client.Customer.User.FirstName.ToLower().Contains(searchTerm) ||
                        p.Client.Customer.User.LastName.ToLower().Contains(searchTerm) ||
                        p.Client.Customer.User.UserName.ToLower().Contains(searchTerm)
                    )) ||

                    // Beneficiary details
                    (p.Beneficiary != null && (
                        p.Beneficiary.BeneficiaryName.ToLower().Contains(searchTerm) ||
                        p.Beneficiary.AccountNumber.ToLower().Contains(searchTerm) ||
                        p.Beneficiary.BankName.ToLower().Contains(searchTerm)
                    )) ||

                    // Client account/bank details
                    (p.Client != null && (
                        p.Client.AccountNumber.ToLower().Contains(searchTerm) ||
                        (p.Client.Bank != null && p.Client.Bank.BankName.ToLower().Contains(searchTerm))
                    )) ||

                    // Payment status
                    (p.PaymentStatus.ToString().ToLower().Contains(searchTerm)) ||

                    // Processed Date (as string)
                    (p.ProcessedDate.HasValue && p.ProcessedDate.Value.ToString().Contains(searchTerm))
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
                        query = isDescending ? query.OrderByDescending(p => p.Client.CompanyName) : query.OrderBy(p => p.Client.CompanyName);
                        break;
                    case "beneficiaryname":
                        query = isDescending ? query.OrderByDescending(p => p.Beneficiary.BeneficiaryName) : query.OrderBy(p => p.Beneficiary.BeneficiaryName);
                        break;
                    case "amount":
                        query = isDescending ? query.OrderByDescending(p => p.Amount) : query.OrderBy(p => p.Amount);
                        break;
                    case "requestdate":
                        query = isDescending ? query.OrderByDescending(p => p.RequestDate) : query.OrderBy(p => p.RequestDate);
                        break;
                    case "processeddate":
                        query = isDescending ? query.OrderByDescending(p => p.ProcessedDate) : query.OrderBy(p => p.ProcessedDate);
                        break;
                    case "paymentstatus":
                        query = isDescending ? query.OrderByDescending(p => p.PaymentStatus) : query.OrderBy(p => p.PaymentStatus);
                        break;
                    default:
                        query = isDescending ? query.OrderByDescending(p => p.RequestDate) : query.OrderBy(p => p.RequestDate);
                        break;
                }
            }
            else
            {
                // Default sort
                query = query.OrderByDescending(p => p.RequestDate);
            }

            // 3. PAGINATION
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Payment>
            {
                Items = items,
                TotalCount = totalCount
            };
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
