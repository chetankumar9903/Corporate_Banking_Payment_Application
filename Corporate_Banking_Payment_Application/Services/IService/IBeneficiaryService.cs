using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IBeneficiaryService
    {
        //Task<IEnumerable<BeneficiaryDto>> GetAllBeneficiaries();
        Task<PagedResult<BeneficiaryDto>> GetAllBeneficiaries(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<BeneficiaryDto?> GetBeneficiaryById(int id);
        Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesByClientId(int clientId);
        Task<BeneficiaryDto> CreateBeneficiary(CreateBeneficiaryDto dto);
        Task<BeneficiaryDto?> UpdateBeneficiary(int id, UpdateBeneficiaryDto dto);
        Task<bool> DeleteBeneficiary(int id);
    }
}
