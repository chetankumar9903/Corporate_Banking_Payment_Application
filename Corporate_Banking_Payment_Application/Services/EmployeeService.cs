using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;
using Corporate_Banking_Payment_Application.Utilities;

namespace Corporate_Banking_Payment_Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepo;

        private readonly IClientRepository _clientRepo;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepo, IClientRepository clientRepo, IMapper mapper)
        {
            _employeeRepo = employeeRepo;
            _clientRepo = clientRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployees()
        {
            var employees = await _employeeRepo.GetAllEmployees();
            // Map the collection of Employee models to EmployeeDto DTOs
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetEmployeeById(int id)
        {
            var employee = await _employeeRepo.GetEmployeeById(id);
            // Map the single Employee model to EmployeeDto DTO, or return null
            return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> CreateEmployee(CreateEmployeeDto dto)
        {

            var client = await _clientRepo.GetClientById(dto.ClientId);
            if (client == null)
                throw new Exception($"Client with ID {dto.ClientId} not found.");

            var bank = client.Bank ?? throw new Exception($"Bank details not found for Client ID {dto.ClientId}.");

            //  Count existing employees for this client (to increment)
            var existingEmployees = await _employeeRepo.GetEmployeesByClientId(dto.ClientId);
            int existingCount = existingEmployees.Count();

            var employee = _mapper.Map<Employee>(dto);

            var created = await _employeeRepo.AddEmployee(employee);


            if (string.IsNullOrWhiteSpace(created.EmployeeCode))
                created.EmployeeCode = EmployeeCodeGenerator.GenerateEmployeeCode(client.CompanyName, client.ClientId, created.EmployeeId);

            // account number using  client bank name
            created.AccountNumber = AccountNumberGenerator.GenerateAccountNumber(bank.BankName, created.EmployeeId);

            await _employeeRepo.UpdateEmployee(created);
            return _mapper.Map<EmployeeDto>(created);
        }

        public async Task<EmployeeDto?> UpdateEmployee(int id, UpdateEmployeeDto dto)
        {
            // 1. Fetch the existing model instance
            var existing = await _employeeRepo.GetEmployeeById(id);
            if (existing == null) return null;

            // 2. Use AutoMapper to apply changes from the DTO to the existing model
            // This handles the partial update logic (null checking) defined in your mapping profile
            _mapper.Map(dto, existing);

            // 3. Save changes to the database
            await _employeeRepo.UpdateEmployee(existing);

            // 4. Map the updated Model back to the DTO for the response
            return _mapper.Map<EmployeeDto>(existing);
        }

        public async Task<bool> DeleteEmployee(int id)
        {
            var employee = await _employeeRepo.GetEmployeeById(id);
            if (employee == null) return false;

            await _employeeRepo.DeleteEmployee(employee);
            return true;
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByClientId(int clientId)
        {
            var client = await _clientRepo.GetClientById(clientId);
            if (client == null)
                throw new Exception($"Client with ID {clientId} not found.");
            var employees = await _employeeRepo.GetEmployeesByClientId(clientId);

            // If no employees found, return empty list (not null)
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }
    }
}
