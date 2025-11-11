using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;
using Corporate_Banking_Payment_Application.Utilities;

namespace Corporate_Banking_Payment_Application.Services
{
    public class ClientService : IClientService
    {

        private readonly IClientRepository _clientRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IBankRepository _bankRepo;
        private readonly IMapper _mapper;

        public ClientService(IClientRepository clientRepo, ICustomerRepository customerRepo, IBankRepository bankRepo, IMapper mapper)
        {
            _clientRepo = clientRepo;
            _customerRepo = customerRepo;
            _bankRepo = bankRepo;
            _mapper = mapper;
        }

        //public async Task<IEnumerable<ClientDto>> GetAllClients()
        //{
        //    var clients = await _clientRepo.GetAllClients();
        //    return _mapper.Map<IEnumerable<ClientDto>>(clients);
        //}
        public async Task<PagedResult<ClientDto>> GetAllClients(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            var pagedResult = await _clientRepo.GetAllClients(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);


            var itemsDto = _mapper.Map<IEnumerable<ClientDto>>(pagedResult.Items);

            return new PagedResult<ClientDto>
            {
                Items = itemsDto.ToList(),
                TotalCount = pagedResult.TotalCount
            };
        }

        public async Task<ClientDto?> GetClientById(int id)
        {
            var client = await _clientRepo.GetClientById(id);
            return _mapper.Map<ClientDto?>(client);
        }

        public async Task<ClientDto> CreateClient(CreateClientDto dto)
        {
            var customer = await _customerRepo.GetCustomerById(dto.CustomerId)
                ?? throw new Exception($"Customer with ID {dto.CustomerId} not found.");

            if (customer.VerificationStatus != Status.APPROVED)
                throw new Exception("Customer must be approved before becoming a client.");

            var bank = await _bankRepo.GetBankById(dto.BankId)
                ?? throw new Exception($"Bank with ID {dto.BankId} not found.");

            var existingClient = await _clientRepo.GetClientByCustomerId(dto.CustomerId);
            if (existingClient != null)
                throw new Exception("This customer is already registered as a client.");



            var client = _mapper.Map<Client>(dto);
            client.AccountNumber = AccountNumberGenerator.GenerateAccountNumber(bank.BankName, 0);


            var createdClient = await _clientRepo.AddClient(client);


            createdClient.AccountNumber = AccountNumberGenerator.GenerateAccountNumber(bank.BankName, createdClient.ClientId);

            await _clientRepo.UpdateClient(createdClient);

            return _mapper.Map<ClientDto>(createdClient);
        }

        public async Task<ClientDto> UpdateClient(int id, UpdateClientDto dto)
        {
            var existing = await _clientRepo.GetClientById(id);
            if (existing == null) throw new Exception("Client not found.");

            _mapper.Map(dto, existing);
            var updated = await _clientRepo.UpdateClient(existing);
            return _mapper.Map<ClientDto>(updated);
        }

        public async Task<bool> DeleteClient(int id)
        {
            return await _clientRepo.DeleteClient(id);
        }


        public async Task<ClientDto?> GetClientByCustomerId(int customerId)
        {
            var client = await _clientRepo.GetClientByCustomerId(customerId);
            return _mapper.Map<ClientDto?>(client);
        }

        public async Task<ClientDto> UpdateClientBalance(int clientId, UpdateClientBalanceDto dto)
        {
            var existing = await _clientRepo.GetClientById(clientId)
                ?? throw new Exception($"Client with ID {clientId} not found.");

            if (!existing.IsActive)
                throw new Exception("Cannot transact on an inactive client account.");


            if (dto.TransactionType == TransactionType.DEPOSIT)
            {

                existing.Balance += dto.Amount;
            }
            else if (dto.TransactionType == TransactionType.WITHDRAW)
            {

                if (existing.Balance < dto.Amount)
                {
                    throw new Exception($"Insufficient funds. Current balance is {existing.Balance:C}, but withdrawal is {dto.Amount:C}.");
                }


                existing.Balance -= dto.Amount;
            }
            else
            {
                throw new Exception("Invalid transaction type specified.");
            }

            var updated = await _clientRepo.UpdateClient(existing);

            return _mapper.Map<ClientDto>(updated);
        }

    }
}
