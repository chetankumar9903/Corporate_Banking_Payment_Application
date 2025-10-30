using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;

namespace Corporate_Banking_Payment_Application.Services
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly IBeneficiaryRepository _beneficiaryRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IMapper _mapper;

        public BeneficiaryService(
            IBeneficiaryRepository beneficiaryRepo,
            IClientRepository clientRepo,
            IMapper mapper)
        {
            _beneficiaryRepo = beneficiaryRepo;
            _clientRepo = clientRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BeneficiaryDto>> GetAllBeneficiaries()
        {
            var beneficiaries = await _beneficiaryRepo.GetAllBeneficiaries();
            return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
        }

        public async Task<BeneficiaryDto?> GetBeneficiaryById(int id)
        {
            var beneficiary = await _beneficiaryRepo.GetBeneficiaryById(id);
            return beneficiary == null ? null : _mapper.Map<BeneficiaryDto>(beneficiary);
        }

        public async Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesByClientId(int clientId)
        {
            var client = await _clientRepo.GetClientById(clientId);
            if (client == null)
                throw new Exception($"Client with ID {clientId} not found.");

            var beneficiaries = await _beneficiaryRepo.GetBeneficiariesByClientId(clientId);
            return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
        }

        public async Task<BeneficiaryDto> CreateBeneficiary(CreateBeneficiaryDto dto)
        {
            var client = await _clientRepo.GetClientById(dto.ClientId);
            if (client == null)
                throw new Exception($"Client with ID {dto.ClientId} not found.");

            var entity = _mapper.Map<Beneficiary>(dto);
            var created = await _beneficiaryRepo.AddBeneficiary(entity);
            return _mapper.Map<BeneficiaryDto>(created);
        }

        public async Task<BeneficiaryDto?> UpdateBeneficiary(int id, UpdateBeneficiaryDto dto)
        {
            var existing = await _beneficiaryRepo.GetBeneficiaryById(id);
            if (existing == null)
                throw new Exception($"Beneficiary with ID {id} not found.");

            _mapper.Map(dto, existing);
            var updated = await _beneficiaryRepo.UpdateBeneficiary(existing);
            return _mapper.Map<BeneficiaryDto>(updated);
        }

        public async Task<bool> DeleteBeneficiary(int id)
        {
            return await _beneficiaryRepo.DeleteBeneficiary(id);
        }
    }
}
