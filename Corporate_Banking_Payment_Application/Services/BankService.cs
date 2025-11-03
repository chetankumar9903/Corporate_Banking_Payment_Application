using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;


namespace Corporate_Banking_Payment_Application.Services
{
    public class BankService : IBankService
    {
        private readonly IBankRepository _bankRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public BankService(IBankRepository bankRepo, IUserRepository userRepo, IMapper mapper)
        {
            _bankRepo = bankRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        //public async Task<IEnumerable<BankDto>> GetAllBank()
        //{
        //    var banks = await _bankRepo.GetAllBank();
        //    return _mapper.Map<IEnumerable<BankDto>>(banks);
        //}
        public async Task<PagedResult<BankDto>> GetAllBank(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            var pagedResult = await _bankRepo.GetAllBank(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);

            // Map the items on the current page to DTOs
            var itemsDto = _mapper.Map<IEnumerable<BankDto>>(pagedResult.Items);

            return new PagedResult<BankDto>
            {
                Items = itemsDto.ToList(),
                TotalCount = pagedResult.TotalCount
            };
        }

        public async Task<BankDto?> GetBankById(int id)
        {
            var bank = await _bankRepo.GetBankById(id);
            return bank == null ? null : _mapper.Map<BankDto>(bank);
        }

        public async Task<BankDto> CreateBank(CreateBankDto dto)
        {
            // Validate User existence
            var userExists = await _userRepo.ExistsUser(dto.UserId);
            if (!userExists)
                throw new Exception($"User with ID {dto.UserId} does not exist.");

            var bank = _mapper.Map<Bank>(dto);
            var created = await _bankRepo.AddBank(bank);
            return _mapper.Map<BankDto>(created);
        }

        public async Task<BankDto?> UpdateBank(int id, UpdateBankDto dto)
        {
            var existing = await _bankRepo.GetBankById(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _bankRepo.UpdateBank(existing);

            return _mapper.Map<BankDto>(existing);
        }

        public async Task<bool> DeleteBank(int id)
        {
            var bank = await _bankRepo.GetBankById(id);
            if (bank == null) return false;

            await _bankRepo.DeleteBank(bank);
            return true;
        }

        public async Task<BankDto?> GetBankByUsername(string username)
        {
            var bank = await _bankRepo.GetBankByUsername(username);
            return _mapper.Map<BankDto?>(bank);
        }
    }
}
