using AutoMapper;
using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;

namespace Corporate_Banking_Payment_Application.Services
{
    public class BatchTransactionService : IBatchTransactionService
    {
        private readonly IBatchTransactionRepository _batchRepo;
        private readonly ISalaryDisbursementRepository _salaryRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BatchTransactionService(
            IBatchTransactionRepository batchRepo,
            ISalaryDisbursementRepository salaryRepo,
            IClientRepository clientRepo,
            IEmployeeRepository employeeRepo,
            AppDbContext context,
            IMapper mapper)
        {
            _batchRepo = batchRepo;
            _salaryRepo = salaryRepo;
            _clientRepo = clientRepo;
            _employeeRepo = employeeRepo;
            _context = context;
            _mapper = mapper;
        }


        public async Task<IEnumerable<BatchTransactionDto>> GetAllBatches()
        {
            var batches = await _batchRepo.GetAll();
            return _mapper.Map<IEnumerable<BatchTransactionDto>>(batches);
        }

        public async Task<BatchTransactionDto?> GetBatchById(int id)
        {
            var batch = await _batchRepo.GetById(id);
            return batch == null ? null : _mapper.Map<BatchTransactionDto>(batch);
        }

        public async Task<IEnumerable<BatchTransactionDto>> GetByClientId(int clientId)
        {
            var list = await _batchRepo.GetByClientId(clientId);
            return _mapper.Map<IEnumerable<BatchTransactionDto>>(list);
        }

        public async Task<BatchTransactionDto> CreateBatch(CreateBatchTransactionDto dto)
        {
            // Validate client
            var client = await _clientRepo.GetClientById(dto.ClientId)
                ?? throw new Exception($"Client with ID {dto.ClientId} not found.");

            if (!client.IsActive)
                throw new Exception("Client account is inactive.");

            var employees = new List<Employee>();
            decimal totalCalculated = 0;

            foreach (var empId in dto.EmployeeIds)
            {
                var emp = await _employeeRepo.GetEmployeeById(empId)
                    ?? throw new Exception($"Employee with ID {empId} not found.");
                if (!emp.IsActive)
                    throw new Exception($"Employee {emp.FirstName} is inactive.");
                if (emp.ClientId != client.ClientId)
                    throw new Exception($"Employee {emp.FirstName} does not belong to this client.");

                totalCalculated += emp.Salary;
                employees.Add(emp);
            }

            // 🧮 Verify provided amount matches calculated total
            if (dto.TotalAmount != totalCalculated)
                throw new Exception($"Provided total ({dto.TotalAmount}) does not match calculated total ({totalCalculated}).");

            if (client.Balance < totalCalculated)
                throw new Exception("Insufficient client balance for batch disbursement.");

            // Ensure atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var batch = new BatchTransaction
                {
                    ClientId = client.ClientId,
                    TotalTransactions = employees.Count,
                    TotalAmount = totalCalculated,
                    Date = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                    )
                };

                client.Balance -= totalCalculated;
                await _clientRepo.UpdateClient(client);

                await _batchRepo.Add(batch);

                foreach (var emp in employees)
                {
                    emp.Balance += emp.Salary;
                    await _employeeRepo.UpdateEmployee(emp);

                    var disbursement = new SalaryDisbursement
                    {
                        ClientId = client.ClientId,
                        EmployeeId = emp.EmployeeId,
                        Amount = emp.Salary,
                        Description = dto.Description ?? "Batch Salary Disbursement",
                        BatchId = batch.BatchId,
                        Date = batch.Date
                    };
                    await _salaryRepo.Add(disbursement);
                }

                await transaction.CommitAsync();

                var created = await _batchRepo.GetById(batch.BatchId);
                return _mapper.Map<BatchTransactionDto>(created);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            //// Map Batch
            //var batch = new BatchTransaction
            //{
            //    ClientId = dto.ClientId,
            //    Date = TimeZoneInfo.ConvertTimeFromUtc(
            //        DateTime.UtcNow,
            //        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
            //    ),
            //    TotalTransactions = dto.Disbursements.Count,
            //    TotalAmount = dto.Disbursements.Sum(d => d.Amount)
            //};

            //// Save batch first
            //var createdBatch = await _batchRepo.Add(batch);

            //// Create salary disbursements under this batch
            //foreach (var disbursementDto in dto.Disbursements)
            //{
            //    var disbursement = _mapper.Map<SalaryDisbursement>(disbursementDto);
            //    disbursement.BatchId = createdBatch.BatchId;
            //    disbursement.Date = batch.Date;

            //    await _salaryRepo.Add(disbursement);
            //}

            //// Reload with included salaries
            //var fullBatch = await _batchRepo.GetById(createdBatch.BatchId);
            //return _mapper.Map<BatchTransactionDto>(fullBatch);
        }

        public async Task<bool> DeleteBatch(int id)
        {
            var existing = await _batchRepo.GetById(id);
            if (existing == null) return false;

            await _batchRepo.Delete(existing);
            return true;
        }
    }
}
