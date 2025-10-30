using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;

namespace Corporate_Banking_Payment_Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;
        private readonly IBankRepository _bankRepo;

        //public CustomerService(ICustomerRepository repo, IMapper mapper)
        //{
        //    _repo = repo;
        //    _mapper = mapper;
        //}

        public CustomerService(ICustomerRepository customerRepo, IUserRepository userRepo, IBankRepository bankRepo, IMapper mapper)
        {
            _customerRepo = customerRepo;
            _userRepo = userRepo;
            _bankRepo = bankRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomers()
        {
            var customers = await _customerRepo.GetAllCustomers();
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetCustomerById(int id)
        {
            var customer = await _customerRepo.GetCustomerById(id);
            return _mapper.Map<CustomerDto?>(customer);
        }

        public async Task<CustomerDto> CreateCustomer(CreateCustomerDto dto)
        {
            //var entity = _mapper.Map<Customer>(dto);
            //var created = await _customerRepo.AddCustomer(entity);
            //return _mapper.Map<CustomerDto>(created);

            // Validate User
            var user = await _userRepo.GetUserById(dto.UserId);
            if (user == null)
                throw new Exception($"User with ID {dto.UserId} does not exist.");

            // Validate Bank
            var bank = await _bankRepo.GetBankById(dto.BankId);
            if (bank == null)
                throw new Exception($"Bank with ID {dto.BankId} does not exist.");

            // Check if user is already linked to another customer
            var existing = await _customerRepo.GetCustomerByUserId(dto.UserId);
            if (existing != null)
                throw new Exception($"User with ID {dto.UserId} is already linked to a Customer record.");

            // Map DTO to Entity
            var customer = _mapper.Map<Customer>(dto);
            customer.VerificationStatus = Status.PENDING;
            customer.IsActive = true;

            //Save
            var created = await _customerRepo.AddCustomer(customer);
            return _mapper.Map<CustomerDto>(created);

        }

        public async Task<CustomerDto> UpdateCustomer(int id, UpdateCustomerDto dto)
        {
            var existing = await _customerRepo.GetCustomerById(id);
            if (existing == null) throw new Exception("Customer not found");

            _mapper.Map(dto, existing);
            var updated = await _customerRepo.UpdateCustomer(existing);
            return _mapper.Map<CustomerDto>(updated);
        }

        public async Task<bool> DeleteCustomer(int id)
        {
            return await _customerRepo.DeleteCustomer(id);
        }


        // ✅ Update Verification Status
        public async Task<CustomerDto> UpdateStatus(int id, Status newStatus)
        {
            var customer = await _customerRepo.GetCustomerById(id);
            if (customer == null)
                throw new Exception($"Customer with ID {id} not found.");

            customer.VerificationStatus = newStatus;
            var updated = await _customerRepo.UpdateCustomer(customer);
            return _mapper.Map<CustomerDto>(updated);
        }


        // ✅ Delete (Soft Delete recommended)
        //public async Task<bool> DeleteAsync(int id)
        //{
        //    var customer = await _customerRepo.GetCustomerById(id);
        //    if (customer == null)
        //        throw new Exception($"Customer with ID {id} not found.");

        //    customer.IsActive = false;
        //    await _customerRepo.UpdateCustomer(customer);
        //    return true;
        //}
    }
}
