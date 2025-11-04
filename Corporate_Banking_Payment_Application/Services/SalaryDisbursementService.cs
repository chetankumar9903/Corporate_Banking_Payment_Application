using AutoMapper;
using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;

namespace Corporate_Banking_Payment_Application.Services
{
    public class SalaryDisbursementService : ISalaryDisbursementService
    {
        private readonly ISalaryDisbursementRepository _repo;
        private readonly IClientRepository _clientRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IBatchTransactionRepository _batchRepo;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SalaryDisbursementService(
             ISalaryDisbursementRepository salaryRepo,
             IClientRepository clientRepo,
             IEmployeeRepository employeeRepo,
             IBatchTransactionRepository batchRepo,
             IMapper mapper,
             AppDbContext context)
        {
            _repo = salaryRepo;
            _clientRepo = clientRepo;
            _employeeRepo = employeeRepo;
            _batchRepo = batchRepo;
            _mapper = mapper;
            _context = context;
        }

        //public async Task<IEnumerable<SalaryDisbursementDto>> GetAll()
        //{
        //    var data = await _repo.GetAll();
        //    return _mapper.Map<IEnumerable<SalaryDisbursementDto>>(data);
        //}

        public async Task<PagedResult<SalaryDisbursementDto>> GetAll(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize, int? clientId)
        {
            var pagedResult = await _repo.GetAll(searchTerm, sortColumn, sortOrder, pageNumber, pageSize, clientId);
            var itemsDto = _mapper.Map<IEnumerable<SalaryDisbursementDto>>(pagedResult.Items);
            return new PagedResult<SalaryDisbursementDto>
            {
                Items = itemsDto.ToList(),
                TotalCount = pagedResult.TotalCount
            };
        }

        public async Task<SalaryDisbursementDto?> GetById(int id)
        {
            var entity = await _repo.GetById(id);
            return entity == null ? null : _mapper.Map<SalaryDisbursementDto>(entity);
        }

        public async Task<IEnumerable<SalaryDisbursementDto>> GetByClientId(int clientId)
        {
            var client = await _clientRepo.GetClientById(clientId)
                ?? throw new Exception($"Client with ID {clientId} not found.");

            var items = await _repo.GetByClientId(clientId);
            return _mapper.Map<IEnumerable<SalaryDisbursementDto>>(items);
        }

        public async Task<IEnumerable<SalaryDisbursementDto>> GetByEmployeeId(int employeeId)
        {
            var employee = await _employeeRepo.GetEmployeeById(employeeId);
            if (employee == null)
                throw new Exception($"Employee with ID {employeeId} not found.");

            var data = await _repo.GetByEmployeeId(employeeId);
            return _mapper.Map<IEnumerable<SalaryDisbursementDto>>(data);
        }

        public async Task<SalaryDisbursementDto> Create(CreateSalaryDisbursementDto dto)
        {
            var client = await _clientRepo.GetClientById(dto.ClientId)
                ?? throw new Exception($"Client with ID {dto.ClientId} not found.");

            if (!client.IsActive)
                throw new Exception("Client account is inactive.");


            var employee = await _employeeRepo.GetEmployeeById(dto.EmployeeId)
                ?? throw new Exception($"Employee with ID {dto.EmployeeId} not found.");

            if (!employee.IsActive)
                throw new Exception("Employee is inactive.");

            if (employee.ClientId != client.ClientId)
                throw new Exception("Employee does not belong to this client.");

            // Validate sufficient balance
            if (client.Balance < dto.Amount)
                throw new Exception("Insufficient client balance to disburse salary.");

            // 4️⃣ If BatchId is provided, validate it
            BatchTransaction? batch = null;
            if (dto.BatchId.HasValue)
            {
                batch = await _batchRepo.GetById(dto.BatchId.Value)
                    ?? throw new Exception($"Batch with ID {dto.BatchId} not found.");
            }


            // Transaction ensures atomic client-employee update
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                client.Balance -= dto.Amount;
                employee.Balance += dto.Amount;

                await _clientRepo.UpdateClient(client);
                await _employeeRepo.UpdateEmployee(employee);

                var salaryDisbursement = new SalaryDisbursement
                {
                    ClientId = dto.ClientId,
                    EmployeeId = dto.EmployeeId,
                    Amount = dto.Amount,
                    Description = dto.Description,
                    BatchId = dto.BatchId,
                    Date = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))
                };

                var created = await _repo.Add(salaryDisbursement);

                if (batch != null)
                {
                    batch.TotalTransactions += 1;
                    batch.TotalAmount += dto.Amount;
                    await _batchRepo.Update(batch);
                }

                await transaction.CommitAsync();
                return _mapper.Map<SalaryDisbursementDto>(created);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<SalaryDisbursementDto?> Update(int id, UpdateSalaryDisbursementDto dto)
        {
            var existing = await _repo.GetById(id);
            if (existing == null)
                throw new Exception($"Salary record with ID {id} not found.");


            if (dto.BatchId.HasValue)
            {
                var batch = await _batchRepo.GetById(dto.BatchId.Value);
                if (batch == null)
                    throw new Exception($"Batch with ID {dto.BatchId.Value} not found.");
            }

            _mapper.Map(dto, existing);

            //var updated = await _repo.Update(existing);
            //return _mapper.Map<SalaryDisbursementDto>(updated);

            await _repo.Update(existing);
            return _mapper.Map<SalaryDisbursementDto>(existing);
        }

        public async Task<bool> Delete(int id)
        {
            //return await _repo.Delete(id);

            var existing = await _repo.GetById(id);
            if (existing == null) return false;

            await _repo.Delete(existing);
            return true;
        }

    }
}
