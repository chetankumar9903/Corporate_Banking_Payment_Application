using AutoMapper;
using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        //public async Task<BatchTransactionDto> CreateBatch(CreateBatchTransactionDto dto)
        //{

        //    var client = await _clientRepo.GetClientById(dto.ClientId)
        //        ?? throw new Exception($"Client with ID {dto.ClientId} not found.");

        //    if (!client.IsActive)
        //        throw new Exception("Client account is inactive.");

        //    var employees = new List<Employee>();
        //    decimal totalCalculated = 0;

        //    foreach (var empId in dto.EmployeeIds)
        //    {
        //        var emp = await _employeeRepo.GetEmployeeById(empId)
        //            ?? throw new Exception($"Employee with ID {empId} not found.");
        //        if (!emp.IsActive)
        //            throw new Exception($"Employee {emp.FirstName} is inactive.");
        //        if (emp.ClientId != client.ClientId)
        //            throw new Exception($"Employee {emp.FirstName} does not belong to this client.");

        //        totalCalculated += emp.Salary;
        //        employees.Add(emp);
        //    }

        //    if (dto.TotalAmount != totalCalculated)
        //        throw new Exception($"Provided total ({dto.TotalAmount}) does not match calculated total ({totalCalculated}).");

        //    if (client.Balance < totalCalculated)
        //        throw new Exception("Insufficient client balance for batch disbursement.");


        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var batch = new BatchTransaction
        //        {
        //            ClientId = client.ClientId,
        //            TotalTransactions = employees.Count,
        //            TotalAmount = totalCalculated,
        //            Date = TimeZoneInfo.ConvertTimeFromUtc(
        //                DateTime.UtcNow,
        //                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
        //            )
        //        };

        //        client.Balance -= totalCalculated;
        //        await _clientRepo.UpdateClient(client);

        //        await _batchRepo.Add(batch);

        //        foreach (var emp in employees)
        //        {
        //            emp.Balance += emp.Salary;
        //            await _employeeRepo.UpdateEmployee(emp);

        //            var disbursement = new SalaryDisbursement
        //            {
        //                ClientId = client.ClientId,
        //                EmployeeId = emp.EmployeeId,
        //                Amount = emp.Salary,
        //                Description = dto.Description ?? "Batch Salary Disbursement",
        //                BatchId = batch.BatchId,
        //                Date = batch.Date
        //            };
        //            await _salaryRepo.Add(disbursement);
        //        }

        //        await transaction.CommitAsync();

        //        var created = await _batchRepo.GetById(batch.BatchId);
        //        return _mapper.Map<BatchTransactionDto>(created);
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }


        //}



        public async Task<object> CreateBatch(CreateBatchTransactionDto dto)
        {

            var client = await _clientRepo.GetClientById(dto.ClientId)
                ?? throw new Exception($"Client with ID {dto.ClientId} not found.");

            if (!client.IsActive)
                throw new Exception("Client account is inactive.");

            var employees = new List<Employee>();
            var skippedAlreadyPaid = new List<string>();
            decimal totalCalculated = 0;

            foreach (var empId in dto.EmployeeIds)
            {
                var emp = await _employeeRepo.GetEmployeeById(empId)
                    ?? throw new Exception($"Employee with ID {empId} not found.");
                if (!emp.IsActive)
                    throw new Exception($"Employee {emp.FirstName} is inactive.");
                if (emp.ClientId != client.ClientId)
                    throw new Exception($"Employee {emp.FirstName} does not belong to this client.");

                if (await HasReceivedSalaryInLast30Days(emp.EmployeeId))
                {
                    skippedAlreadyPaid.Add($"{emp.FirstName} {emp.LastName} (Code: {emp.EmployeeCode})");
                    continue;
                }

                totalCalculated += emp.Salary;
                employees.Add(emp);
            }
            if (employees.Count == 0)
            {
                throw new Exception("No eligible employees for payment. All selected employees were paid within the last 30 days.");
            }


            //if (dto.TotalAmount != totalCalculated)
            //    throw new Exception($"Provided total ({dto.TotalAmount}) does not match calculated total ({totalCalculated}).");

            dto.TotalAmount = totalCalculated;

            if (client.Balance < totalCalculated)
                throw new Exception("Insufficient client balance for batch disbursement.");


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
                //return _mapper.Map<BatchTransactionDto>(created);
                return new
                {
                    success = true,
                    batch = _mapper.Map<BatchTransactionDto>(created),
                    skippedAlreadyPaid
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<bool> DeleteBatch(int id)
        {
            var existing = await _batchRepo.GetById(id);
            if (existing == null) return false;

            await _batchRepo.Delete(existing);
            return true;
        }

        //[Authorize(Roles = "CLIENTUSER")]
        //[HttpPost("upload-csv")]
        //public async Task<object> ProcessBatchCsv(IFormFile file, [FromQuery] int clientId)
        //{
        //    var client = await _clientRepo.GetClientById(clientId);
        //    if (client == null)
        //        throw new Exception("Client not found.");

        //    using var stream = new StreamReader(file.OpenReadStream());
        //    List<string> employeeCodes = new();
        //    string? line;
        //    int lineNo = 0;

        //    while ((line = await stream.ReadLineAsync()) != null)
        //    {
        //        lineNo++;
        //        if (lineNo == 1) continue; // Skip header
        //        if (string.IsNullOrWhiteSpace(line)) continue;

        //        employeeCodes.Add(line.Trim());
        //    }

        //    if (employeeCodes.Count == 0)
        //        throw new Exception("No employee codes found in CSV.");

        //    // Get all employees for the client
        //    var employees = await _employeeRepo.GetEmployeesByClientId(clientId);

        //    // Valid employees = match code & active
        //    var validEmployees = employees
        //        .Where(e => employeeCodes.Contains(e.EmployeeCode) && e.IsActive)
        //        .ToList();

        //    // Invalid = codes that didn't match or inactive
        //    var invalidEmployees = employeeCodes
        //        .Where(code => !validEmployees.Any(v => v.EmployeeCode == code))
        //        .ToList();

        //    if (validEmployees.Count == 0)
        //        throw new Exception("No valid active employees found in CSV.");

        //    // Calculate total salary amount
        //    decimal totalAmount = validEmployees.Sum(e => e.Salary);

        //    if (client.Balance < totalAmount)
        //        throw new Exception("Insufficient client balance.");

        //    // Create batch record
        //    var batch = new BatchTransaction
        //    {
        //        ClientId = clientId,
        //        TotalTransactions = validEmployees.Count,
        //        TotalAmount = totalAmount,
        //        Date = TimeZoneInfo.ConvertTimeFromUtc(
        //            DateTime.UtcNow,
        //            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
        //        )
        //    };
        //    await _batchRepo.Add(batch);

        //    // Perform salary disbursement
        //    foreach (var emp in validEmployees)
        //    {
        //        emp.Balance += emp.Salary;
        //        await _employeeRepo.UpdateEmployee(emp);

        //        await _salaryRepo.Add(new SalaryDisbursement
        //        {
        //            ClientId = clientId,
        //            EmployeeId = emp.EmployeeId,
        //            Amount = emp.Salary,
        //            Description = "Batch CSV Salary Disbursement",
        //            BatchId = batch.BatchId,
        //            Date = batch.Date
        //        });
        //    }

        //    // Deduct from client
        //    client.Balance -= totalAmount;
        //    await _clientRepo.UpdateClient(client);

        //    return new
        //    {
        //        created = validEmployees.Count,
        //        skipped = invalidEmployees.Count,
        //        skippedEmployees = invalidEmployees
        //    };
        //}


        [Authorize(Roles = "CLIENTUSER")]
        [HttpPost("upload-csv")]
        public async Task<object> ProcessBatchCsv(IFormFile file, [FromQuery] int clientId)
        {
            var client = await _clientRepo.GetClientById(clientId);
            if (client == null)
                throw new Exception("Client not found.");

            using var stream = new StreamReader(file.OpenReadStream());
            List<string> employeeCodes = new();
            string? line;
            int lineNo = 0;

            while ((line = await stream.ReadLineAsync()) != null)
            {
                lineNo++;
                if (lineNo == 1) continue;
                if (string.IsNullOrWhiteSpace(line)) continue;

                employeeCodes.Add(line.Trim());
            }

            if (employeeCodes.Count == 0)
                throw new Exception("No employee codes found in CSV.");

            var employees = await _employeeRepo.GetEmployeesByClientId(clientId);

            var validEmployees = new List<Employee>();
            var skippedAlreadyPaid = new List<string>();

            foreach (var emp in employees.Where(e => employeeCodes.Contains(e.EmployeeCode) && e.IsActive))
            {
                if (await HasReceivedSalaryInLast30Days(emp.EmployeeId))
                {
                    skippedAlreadyPaid.Add($"{emp.EmployeeCode} - {emp.FirstName} {emp.LastName}");
                    continue;
                }

                validEmployees.Add(emp);
            }


            var invalidEmployees = employeeCodes
                .Where(code => !employees.Any(e => e.EmployeeCode == code && e.IsActive))
                .ToList();

            if (validEmployees.Count == 0)
                throw new Exception("No valid employees eligible for payment (all recently paid or invalid).");

            decimal totalAmount = validEmployees.Sum(e => e.Salary);
            if (client.Balance < totalAmount)
                throw new Exception("Insufficient client balance.");

            var batch = new BatchTransaction
            {
                ClientId = clientId,
                TotalTransactions = validEmployees.Count,
                TotalAmount = totalAmount,
                Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))
            };
            await _batchRepo.Add(batch);

            foreach (var emp in validEmployees)
            {
                emp.Balance += emp.Salary;
                await _employeeRepo.UpdateEmployee(emp);

                await _salaryRepo.Add(new SalaryDisbursement
                {
                    ClientId = clientId,
                    EmployeeId = emp.EmployeeId,
                    Amount = emp.Salary,
                    Description = "Batch CSV Salary Disbursement",
                    BatchId = batch.BatchId,
                    Date = batch.Date
                });
            }

            client.Balance -= totalAmount;
            await _clientRepo.UpdateClient(client);

            return new
            {
                created = validEmployees.Count,
                skippedInvalid = invalidEmployees.Count,
                skippedAlreadyPaid = skippedAlreadyPaid.Count,
                invalidEmployees,
                alreadyPaid = skippedAlreadyPaid
            };
        }

        //public async Task<bool> HasReceivedSalaryInLast30Days(int employeeId)
        //{
        //    var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        //    return await _context.SalaryDisbursements
        //        .AnyAsync(s => s.EmployeeId == employeeId && s.Date >= thirtyDaysAgo);
        //}

        public async Task<bool> HasReceivedSalaryInLast30Days(int employeeId)
        {
            var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            var currentIstTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istZone);

            var thirtyDaysAgoIst = currentIstTime.AddDays(-30);

            return await _context.SalaryDisbursements
                .AnyAsync(s => s.EmployeeId == employeeId && s.Date >= thirtyDaysAgoIst);
        }




    }
}
