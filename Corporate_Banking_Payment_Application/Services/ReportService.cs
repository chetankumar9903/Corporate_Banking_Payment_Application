using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;
using Corporate_Banking_Payment_Application.Utilities;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Corporate_Banking_Payment_Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepo;
        private readonly IUserRepository _userRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ISalaryDisbursementRepository _salaryRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IReportRepository reportRepo,
            IUserRepository userRepo,
            IPaymentRepository paymentRepo,
            ISalaryDisbursementRepository salaryRepo,
            IClientRepository clientRepo,
            IMapper mapper,
            Cloudinary cloudinary,
            ILogger<ReportService> logger)
        {
            _reportRepo = reportRepo;
            _userRepo = userRepo;
            _paymentRepo = paymentRepo;
            _salaryRepo = salaryRepo;
            _clientRepo = clientRepo;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _logger = logger;
        }

        public async Task<ReportDto> GenerateAndSaveReport(GenerateReportRequestDto request, int currentUserId, UserRole currentUserRole)
        {
            // 1. Validate and Authorize Request (Same as before)
            var user = await ValidateAndAuthorizeRequest(request, currentUserId, currentUserRole);

            if (!request.ClientId.HasValue)
            {
                throw new ArgumentException("A ClientId must be provided for this report type.");
            }

            var clientId = request.ClientId.Value;
            MemoryStream fileStream;

            // 2. Fetch Data AND Generate File (This replaces your GetReportData method)
            switch (request.ReportType)
            {
                case ReportType.PAYMENT:
                    {

                        var payments = await _paymentRepo.GetPaymentsByClientId(clientId);
                        var filteredPayments = ApplyDateRangeFilter(payments, request.StartDate, request.EndDate);

                        if (!filteredPayments.Any())
                        {
                            throw new ArgumentException("No payment data found for the selected criteria.");
                        }

                        var paymentReportData = filteredPayments.Select(p => new PaymentReportDto
                        {
                            PaymentId = p.PaymentId,
                            ClientId = p.ClientId,
                            BeneficiaryId = p.BeneficiaryId,
                            Amount = p.Amount,
                            RequestDate = p.RequestDate,
                            ProcessedDate = p.ProcessedDate,
                            PaymentStatus = p.PaymentStatus,
                            Description = p.Description,
                            RejectReason = p.RejectReason
                        }).ToList();

                        // Pass the new DTO list to the generator
                        fileStream = ReportGenerator.Generate(paymentReportData, request.OutputFormat, request.ReportType);
                        break;
                    }

                case ReportType.SALARY:
                    {
                        var salaries = await _salaryRepo.GetByClientId(clientId);
                        var filteredSalaries = ApplyDateRangeFilter(salaries, request.StartDate, request.EndDate);

                        if (!filteredSalaries.Any())
                        {
                            throw new ArgumentException("No salary data found for the selected criteria.");
                        }

                        var salaryReportData = filteredSalaries.Select(s => new SalaryReportDto
                        {
                            SalaryDisbursementId = s.SalaryDisbursementId,
                            ClientId = s.ClientId,
                            EmployeeId = s.EmployeeId,
                            Amount = s.Amount,
                            Date = s.Date,
                            Description = s.Description,
                            BatchId = s.BatchId
                        }).ToList();

                        // Pass the new DTO list to the generator
                        fileStream = ReportGenerator.Generate(salaryReportData, request.OutputFormat, request.ReportType);
                        break;
                    }

                case ReportType.TRANSACTION:
                    {
                        var allPayments = await _paymentRepo.GetPaymentsByClientId(clientId);
                        var allSalaries = await _salaryRepo.GetByClientId(clientId);

                        var combinedTransactions = new List<TransactionReportDto>();

                        // Map Payments
                        combinedTransactions.AddRange(allPayments.Select(p => new TransactionReportDto
                        {
                            TransactionId = $"PAY-{p.PaymentId}",
                            Date = p.RequestDate,
                            Type = "Payment",
                            Amount = p.Amount,
                            // Check if Beneficiary was loaded
                            Recipient = p.Beneficiary?.BeneficiaryName ?? "N/A",
                            Status = p.PaymentStatus.ToString(),
                            Description = p.Description
                        }));

                        // Map Salary Disbursements
                        combinedTransactions.AddRange(allSalaries.Select(s => new TransactionReportDto
                        {
                            TransactionId = $"SAL-{s.SalaryDisbursementId}",
                            Date = s.Date,
                            Type = "Salary",
                            Amount = s.Amount,
                            // Check if Employee was loaded
                            Recipient = s.Employee != null ? $"{s.Employee.FirstName} {s.Employee.LastName}" : "N/A",
                            Status = "Disbursed", // Salary disbursements are considered complete
                            Description = s.Description
                        }));

                        var filteredTransactions = ApplyDateRangeFilter(combinedTransactions, request.StartDate, request.EndDate)
                                                     .OrderBy(t => t.Date) // Order by date
                                                     .ToList();

                        if (!filteredTransactions.Any())
                        {
                            throw new ArgumentException("No transaction data found for the selected criteria.");
                        }

                        fileStream = ReportGenerator.Generate(filteredTransactions, request.OutputFormat, request.ReportType);
                        break;
                    }

                default:
                    throw new ArgumentException($"Unsupported ReportType: {request.ReportType}");
            }



            // 1. Get the raw bytes from the stream
            // (Make sure your ReportGenerator code includes stream.Position = 0!)
            byte[] fileBytes = fileStream.ToArray();
            fileStream.Close();
            fileStream.Dispose();
            // 2. Define a local path to save the file
            string fileExtension = request.OutputFormat == ReportOutputFormat.EXCEL ? "xlsx" : "pdf";
            string localFilePath = Path.Combine(@"C:\TempReports", $"{request.ReportName}-{DateTime.Now.Ticks}.{fileExtension}");

            // 3. Save the file to your local disk
            try
            {
                await File.WriteAllBytesAsync(localFilePath, fileBytes);
                _logger.LogInformation("Successfully saved local test report to: {LocalPath}", localFilePath);
            }
            catch (Exception ex)
            {
                // Log a warning, but don't stop the real upload
                _logger.LogWarning(ex, "Failed to save local test report to: {LocalPath}", localFilePath);
            }


            // 5. Upload and Save
            // Create a NEW, clean stream from the byte array for Cloudinary
            using (var uploadStream = new MemoryStream(fileBytes))
            {
                // Pass the NEW stream to the uploader
                var uploadResult = await UploadReportToCloudinary(uploadStream, request.ReportName, request.OutputFormat);

                // 6. Save Report Record
                var reportToCreate = new Report
                {
                    ReportName = request.ReportName,
                    ReportType = request.ReportType,
                    GeneratedBy = currentUserId,
                    OutputFormat = request.OutputFormat,
                    FilePath = uploadResult.SecureUrl.ToString()
                };

                var created = await _reportRepo.AddReport(reportToCreate);
                return _mapper.Map<ReportDto>(created);
            }
        }

        public async Task<ReportDto?> GetReportById(int reportId)
        {
            var report = await _reportRepo.GetReportById(reportId);
            return report == null ? null : _mapper.Map<ReportDto>(report);
        }

        //public async Task<IEnumerable<ReportDto>> GetReportsByUser(int userId)
        //{
        //    // The repository call can remain the same
        //    var reports = await _reportRepo.GetReportsByUserId(userId);
        //    return _mapper.Map<IEnumerable<ReportDto>>(reports);
        //}

        public async Task<PagedResult<ReportDto>> GetReportsByUser(int userId, string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            var pagedResult = await _reportRepo.GetReportsByUserId(userId, searchTerm, sortColumn, sortOrder, pageNumber, pageSize);

            var itemsDto = _mapper.Map<IEnumerable<ReportDto>>(pagedResult.Items);

            return new PagedResult<ReportDto>
            {
                Items = itemsDto.ToList(),
                TotalCount = pagedResult.TotalCount
            };
        }

        // --- Internal Helper Methods ---

        // Signature updated to accept UserRole
        private async Task<User> ValidateAndAuthorizeRequest(GenerateReportRequestDto request, int userId, UserRole currentUserRole)
        {
            var user = await _userRepo.GetUserById(userId)
                ?? throw new Exception("Generating user not found.");

            // Verify the passed role matches the user's actual role
            if (user.UserRole != currentUserRole)
            {
                throw new UnauthorizedAccessException("User role mismatch.");
            }

            if (request.ClientId.HasValue)
            {
                var client = await _clientRepo.GetClientById(request.ClientId.Value);
                if (client == null)
                    throw new Exception($"Client ID {request.ClientId.Value} specified in request is invalid.");
            }

            switch (currentUserRole) // Use the validated role
            {
                case UserRole.SUPERADMIN:
                    break;

                case UserRole.BANKUSER:
                    break;

                case UserRole.CLIENTUSER:
                    var customerId = user.Customer?.CustomerId ?? throw new Exception("Client User not linked to a Customer account.");

                    var clientUser = await _clientRepo.GetClientByCustomerId(customerId)
                        ?? throw new Exception("Client User's Customer not linked to a Client account.");

                    if (request.ClientId.HasValue && request.ClientId.Value != clientUser.ClientId)
                    {
                        throw new UnauthorizedAccessException("Client User cannot generate reports for other clients.");
                    }
                    request.ClientId = clientUser.ClientId;
                    break;

                default:
                    throw new UnauthorizedAccessException("User role not authorized for report generation.");
            }

            return user;
        }

        private async Task<RawUploadResult> UploadReportToCloudinary(Stream fileStream, string reportName, ReportOutputFormat format)
        {
            string fileExtension = format == ReportOutputFormat.EXCEL ? "xlsx" : "pdf";
            string publicId = $"reports/{reportName.Replace(" ", "-")}-{DateTime.UtcNow.Ticks}";

            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription($"{publicId}.{fileExtension}", fileStream),
                PublicId = publicId,
                Folder = "corporate_banking_app_reports",
                Type = "upload"
            };

            RawUploadResult uploadResult;
            try
            {
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload report to Cloudinary for report: {ReportName}", reportName);
                throw new Exception("Report file upload failed due to external service error.", ex);
            }

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError("Cloudinary upload failed with status: {Status} and message: {Message}",
                    uploadResult.StatusCode, uploadResult.Error?.Message);
                throw new Exception($"Cloudinary report upload failed: {uploadResult.Error?.Message}");
            }

            return uploadResult;
        }

        private IEnumerable<T> ApplyDateRangeFilter<T>(IEnumerable<T> data, DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue && !endDate.HasValue)
                return data;

            var dateProperty = typeof(T).GetProperty("RequestDate");

            if (dateProperty == null)
            {
                dateProperty = typeof(T).GetProperty("Date");
            }

            if (dateProperty == null || (dateProperty.PropertyType != typeof(DateTime) && dateProperty.PropertyType != typeof(DateTime?)))
            {
                _logger.LogWarning($"Cannot apply date filter: No suitable 'RequestDate' or 'Date' property found on type {typeof(T).Name}.");
                return data;
            }

            return data.Where(item =>
            {
                var dateValue = (DateTime?)dateProperty.GetValue(item);
                if (!dateValue.HasValue) return false;

                bool afterStart = !startDate.HasValue || dateValue.Value.Date >= startDate.Value.Date;
                bool beforeEnd = !endDate.HasValue || dateValue.Value.Date <= endDate.Value.Date;

                return afterStart && beforeEnd;
            });
        }
    }
}
