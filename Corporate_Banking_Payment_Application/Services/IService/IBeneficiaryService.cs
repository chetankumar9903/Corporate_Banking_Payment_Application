using Corporate_Banking_Payment_Application.DTOs;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IBeneficiaryService
    {
        Task<IEnumerable<BeneficiaryDto>> GetAllBeneficiaries();
        Task<BeneficiaryDto?> GetBeneficiaryById(int id);
        Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesByClientId(int clientId);
        Task<BeneficiaryDto> CreateBeneficiary(CreateBeneficiaryDto dto);
        Task<BeneficiaryDto?> UpdateBeneficiary(int id, UpdateBeneficiaryDto dto);
        Task<bool> DeleteBeneficiary(int id);
    }
}
