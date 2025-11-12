using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface IPaymentRepository
    {

        //Task<IEnumerable<Payment>> GetAllPayments();
        Task<PagedResult<Payment>> GetAllPayments(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<Payment?> GetPaymentById(int id);
        Task<Payment> AddPayment(Payment payment);
        Task UpdatePayment(Payment payment);
        Task DeletePayment(Payment payment);


        Task<bool> ExistsPayment(int id);
        Task<IEnumerable<Payment>> GetPaymentsByClientId(int clientId);

        Task<IEnumerable<Payment>> GetPaymentsByBeneficiaryId(int beneficiaryId);
        Task<IEnumerable<Payment>> GetPaymentsByStatus(Status status);
    }
}
