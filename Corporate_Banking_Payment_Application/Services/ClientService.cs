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

        public async Task<IEnumerable<ClientDto>> GetAllClients()
        {
            var clients = await _clientRepo.GetAllClients();
            return _mapper.Map<IEnumerable<ClientDto>>(clients);
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

            //var client = _mapper.Map<Client>(dto);
            ////client.GenerateAccountNumber();

            ////  Generate unique account number using utility
            //client.AccountNumber = AccountNumberGenerator.GenerateAccountNumber(bank.BankName.Substring(0, 2).ToUpper());

            //var created = await _clientRepo.AddClient(client);
            //return _mapper.Map<ClientDto>(created);


            // 1 Map DTO to entity
            var client = _mapper.Map<Client>(dto);

            // 2 Save first to get ClientId
            var createdClient = await _clientRepo.AddClient(client);

            // 3 Generate account number using Bank + ClientId
            createdClient.AccountNumber = AccountNumberGenerator.GenerateAccountNumber(bank.BankName, createdClient.ClientId);

            // 4 Update client with the generated account number
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

        // ✅ Last Method: Get client by CustomerId
        public async Task<ClientDto?> GetClientByCustomerId(int customerId)
        {
            var client = await _clientRepo.GetClientByCustomerId(customerId);
            return _mapper.Map<ClientDto?>(client);
        }

    }
}
