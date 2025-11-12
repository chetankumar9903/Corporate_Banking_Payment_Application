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

        private readonly IEmailService _emailService;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            ICustomerRepository customerRepo,
            IUserRepository userRepo,
            IBankRepository bankRepo,
            IMapper mapper,

            IEmailService emailService,
            ILogger<CustomerService> logger
        )
        {
            _customerRepo = customerRepo;
            _userRepo = userRepo;
            _bankRepo = bankRepo;
            _mapper = mapper;

            _emailService = emailService;
            _logger = logger;
        }



        public async Task<PagedResult<CustomerDto>> GetAllCustomers(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            var pagedResult = await _customerRepo.GetAllCustomers(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            var itemsDto = _mapper.Map<IEnumerable<CustomerDto>>(pagedResult.Items);
            return new PagedResult<CustomerDto>
            {
                Items = itemsDto.ToList(),
                TotalCount = pagedResult.TotalCount
            };
        }

        public async Task<CustomerDto?> GetCustomerById(int id)
        {
            var customer = await _customerRepo.GetCustomerById(id);
            return _mapper.Map<CustomerDto?>(customer);
        }

        public async Task<CustomerDto> CreateCustomer(CreateCustomerDto dto)
        {
            var user = await _userRepo.GetUserById(dto.UserId);
            if (user == null)
                throw new Exception($"User with ID {dto.UserId} does not exist.");

            var bank = await _bankRepo.GetBankById(dto.BankId);
            if (bank == null)
                throw new Exception($"Bank with ID {dto.BankId} does not exist.");

            var existing = await _customerRepo.GetCustomerByUserId(dto.UserId);
            if (existing != null)
                throw new Exception($"User with ID {dto.UserId} is already linked to a Customer record.");

            var customer = _mapper.Map<Customer>(dto);
            customer.VerificationStatus = Status.PENDING;
            customer.IsActive = true;

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


        public async Task<CustomerDto> UpdateStatus(int id, Status newStatus)
        {
            var customer = await _customerRepo.GetCustomerById(id);
            if (customer == null)
                throw new Exception($"Customer with ID {id} not found.");

            string customerEmail = customer.User.EmailId;
            string customerName = customer.User.FirstName;

            customer.VerificationStatus = newStatus;
            var updated = await _customerRepo.UpdateCustomer(customer);


            try
            {
                string subject = "";
                string body = "";

                if (newStatus == Status.APPROVED)
                {
                    subject = "Your Application has been Approved!";
                    body = $"<p>Dear {customerName},</p>" +
                           $"<p>Congratulations! Your application with our bank has been <strong>APPROVED</strong>.</p>" +
                           "<p>You can now log in to the client portal to set up your account.</p>" +
                           "<p>Thank you,<br/>The Corporate Banking Team</p>";
                }
                else if (newStatus == Status.REJECTED)
                {
                    subject = "Your Application has been Rejected";
                    body = $"<p>Dear {customerName},</p>" +
                           $"<p>We regret to inform you that your application with our bank has been <strong>REJECTED</strong>.</p>" +
                           "<p>Please contact your bank representative for more details on how to re-submit your documents.</p>" +
                           "<p>Thank you,<br/>The Corporate Banking Team</p>";
                }


                if (!string.IsNullOrEmpty(subject))
                {
                    await _emailService.SendEmailAsync(customerEmail, subject, body);
                    _logger.LogInformation($"Successfully sent status update email to {customerEmail}");
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Failed to send email to {customerEmail} for status update.");
            }


            return _mapper.Map<CustomerDto>(updated);
        }
    }
}