using Corporate_Banking_Payment_Application.DTOs;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IBankService
    {
        Task<IEnumerable<BankDto>> GetAllBank();
        Task<BankDto?> GetBankById(int id);
        Task<BankDto> CreateBank(CreateBankDto dto);
        Task<BankDto?> UpdateBank(int id, UpdateBankDto dto);
        Task<bool> DeleteBank(int id);
    }
}
