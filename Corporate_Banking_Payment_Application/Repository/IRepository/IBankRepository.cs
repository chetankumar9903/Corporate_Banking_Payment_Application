using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface IBankRepository
    {
        //Task<IEnumerable<Bank>> GetAllBank();
        Task<PagedResult<Bank>> GetAllBank(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<Bank?> GetBankById(int id);
        Task<Bank> AddBank(Bank bank);
        Task UpdateBank(Bank bank);
        Task DeleteBank(Bank bank);
        Task<bool> ExistsBank(int id);
        Task<Bank?> GetBankByUsername(string username);
    }
}
