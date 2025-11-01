using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface IBeneficiaryRepository
    {
        //Task<IEnumerable<Beneficiary>> GetAllBeneficiaries();
        Task<PagedResult<Beneficiary>> GetAllBeneficiaries(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<Beneficiary?> GetBeneficiaryById(int id);
        Task<IEnumerable<Beneficiary>> GetBeneficiariesByClientId(int clientId);
        Task<Beneficiary> AddBeneficiary(Beneficiary beneficiary);
        Task<Beneficiary> UpdateBeneficiary(Beneficiary beneficiary);
        Task<bool> DeleteBeneficiary(int id);
    }
}
