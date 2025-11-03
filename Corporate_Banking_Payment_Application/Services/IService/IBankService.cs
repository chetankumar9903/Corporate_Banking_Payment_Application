using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IBankService
    {
        //Task<IEnumerable<BankDto>> GetAllBank();
        Task<PagedResult<BankDto>> GetAllBank(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<BankDto?> GetBankById(int id);
        Task<BankDto> CreateBank(CreateBankDto dto);
        Task<BankDto?> UpdateBank(int id, UpdateBankDto dto);
        Task<bool> DeleteBank(int id);

        Task<BankDto?> GetBankByUsername(string username);
    }
}
