using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IPaymentService
    {

        //Task<IEnumerable<PaymentDto>> GetAllPayments();
        Task<PagedResult<PaymentDto>> GetAllPayments(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<PaymentDto?> GetPaymentById(int id);
        Task<PaymentDto> CreatePayment(CreatePaymentDto dto);
        Task<PaymentDto?> UpdatePayment(int id, UpdatePaymentDto dto);
        Task<bool> DeletePayment(int id);


        Task<IEnumerable<PaymentDto>> GetPaymentsByClientId(int clientId);
        Task<IEnumerable<PaymentDto>> GetPaymentsByBeneficiaryId(int beneficiaryId);
        Task<IEnumerable<PaymentDto>> GetPaymentsByStatus(Status status);
    }
}

