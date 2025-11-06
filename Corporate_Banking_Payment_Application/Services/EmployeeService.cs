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

        //public async Task<IEnumerable<EmployeeDto>> GetAllEmployees()
        //{
        //    var employees = await _employeeRepo.GetAllEmployees();
        //    // Map the collection of Employee models to EmployeeDto DTOs
        //    return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        //}

        public async Task<PagedResult<EmployeeDto>> GetAllEmployees(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            var pagedResult = await _employeeRepo.GetAllEmployees(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);

            var itemsDto = _mapper.Map<IEnumerable<EmployeeDto>>(pagedResult.Items);

            return new PagedResult<EmployeeDto>
            {
                Items = itemsDto.ToList(),
                TotalCount = pagedResult.TotalCount
            };
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
            // Pre-generate unique EmployeeCode and AccountNumber
            employee.EmployeeCode = EmployeeCodeGenerator.GenerateEmployeeCode(client.CompanyName, client.ClientId, 0); // placeholder
            employee.AccountNumber = AccountNumberGenerator.GenerateAccountNumber(bank.BankName, 0); // placeholder

            var created = await _employeeRepo.AddEmployee(employee);


            //if (string.IsNullOrWhiteSpace(created.EmployeeCode))
            //    created.EmployeeCode = EmployeeCodeGenerator.GenerateEmployeeCode(client.CompanyName, client.ClientId, created.EmployeeId);

            //// account number using  client bank name
            //created.AccountNumber = AccountNumberGenerator.GenerateAccountNumber(bank.BankName, created.EmployeeId);

            // Update with EmployeeId if needed
            created.EmployeeCode = EmployeeCodeGenerator.GenerateEmployeeCode(client.CompanyName, client.ClientId, created.EmployeeId);
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

            //await _employeeRepo.DeleteEmployee(employee);
            //employee.IsActive = false;
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



        //public async Task<object> ProcessEmployeeCsv(IFormFile file, int clientId)
        //{
        //    var client = await _clientRepo.GetClientById(clientId);
        //    if (client == null)
        //        throw new Exception("Client not found.");

        //    using var stream = new StreamReader(file.OpenReadStream());
        //    List<CreateEmployeeDto> parsedEmployees = new();
        //    string? line;
        //    int lineNo = 0;

        //    while ((line = await stream.ReadLineAsync()) != null)
        //    {
        //        lineNo++;
        //        if (lineNo == 1) continue; // skip header

        //        var cols = line.Split(',');
        //        //if (cols.Length < 4)
        //        //    continue;
        //        if (cols.Length < 7) continue;

        //        parsedEmployees.Add(new CreateEmployeeDto
        //        {
        //            ClientId = clientId,
        //            FirstName = cols[0].Trim(),
        //            LastName = cols[1].Trim(),
        //            EmailId = cols[2].Trim(),
        //            PhoneNumber = cols[3].Trim(),
        //            Position = cols[4].Trim(),
        //            Department = cols[5].Trim(),
        //            Salary = decimal.TryParse(cols[6], out var sal) ? sal : 0,
        //            IsActive = cols.Length > 7 ? cols[7].Trim().ToLower() == "true" : true

        //        });
        //    }

        //    var existingEmployees = await _employeeRepo.GetEmployeesByClientId(clientId);
        //    int created = 0, skipped = 0;

        //    foreach (var emp in parsedEmployees)
        //    {
        //        bool exists = existingEmployees.Any(e =>
        //            e.FirstName.ToLower() == emp.FirstName.ToLower() &&
        //            e.LastName?.ToLower() == emp.LastName?.ToLower() &&
        //            e.EmailId.ToLower() == emp.EmailId.ToLower() &&
        //            e.PhoneNumber == emp.PhoneNumber
        //        );

        //        if (exists)
        //        {
        //            skipped++;
        //            continue;
        //        }

        //        await CreateEmployee(emp); // Reuse existing logic
        //        created++;
        //    }

        //    return new { created, skipped };
        //}

        public async Task<object> ProcessEmployeeCsv(IFormFile file, int clientId)
        {
            var client = await _clientRepo.GetClientById(clientId);
            if (client == null)
                throw new Exception("Client not found.");

            using var stream = new StreamReader(file.OpenReadStream());
            List<CreateEmployeeDto> parsedEmployees = new();
            string? line;
            int lineNo = 0;

            while ((line = await stream.ReadLineAsync()) != null)
            {
                lineNo++;
                if (lineNo == 1) continue;

                var cols = line.Split(',');
                if (cols.Length < 7) continue;

                parsedEmployees.Add(new CreateEmployeeDto
                {
                    ClientId = clientId,
                    FirstName = cols[0].Trim(),
                    LastName = cols[1].Trim(),
                    EmailId = cols[2].Trim(),
                    PhoneNumber = cols[3].Trim(),
                    Position = cols[4].Trim(),
                    Department = cols[5].Trim(),
                    Salary = decimal.TryParse(cols[6], out var sal) ? sal : 0,
                    IsActive = cols.Length > 7 ? cols[7].Trim().ToLower() == "true" : true
                });
            }

            var existingEmployees = await _employeeRepo.GetEmployeesByClientId(clientId);

            int created = 0;
            List<string> errors = new();

            foreach (var emp in parsedEmployees)
            {
                var samePhone = existingEmployees.FirstOrDefault(e => e.PhoneNumber == emp.PhoneNumber);
                if (samePhone != null)
                {
                    errors.Add($"Phone Number '{emp.PhoneNumber}' already exists for {samePhone.FirstName} {samePhone.LastName}.");
                    continue;
                }

                var sameEmail = existingEmployees.FirstOrDefault(e => e.EmailId.ToLower() == emp.EmailId.ToLower());
                if (sameEmail != null)
                {
                    errors.Add($"Email '{emp.EmailId}' already exists for {sameEmail.FirstName} {sameEmail.LastName}.");
                    continue;
                }

                var samePerson = existingEmployees.FirstOrDefault(e =>
                    e.FirstName.ToLower() == emp.FirstName.ToLower() &&
                    e.LastName.ToLower() == emp.LastName.ToLower()
                );

                if (samePerson != null)
                {
                    errors.Add($"Duplicate Employee: '{emp.FirstName} {emp.LastName}' already exists.");
                    continue;
                }

                await CreateEmployee(emp);
                created++;
            }

            return new
            {
                created,
                errorsCount = errors.Count,
                errors
            };
        }

    }

}
