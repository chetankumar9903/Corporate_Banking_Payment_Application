//using AutoMapper;
//using CloudinaryDotNet;
//using CloudinaryDotNet.Actions;
//using corporate_banking_payment_application.DTOs;
//using Corporate_Banking_Payment_Application.DTOs;
//using Corporate_Banking_Payment_Application.Models;
//using Corporate_Banking_Payment_Application.Repository.IRepository;
//using Corporate_Banking_Payment_Application.Services.IService;
//using Corporate_Banking_Payment_Application.Utilities;
//using Microsoft.Extensions.Logging;
//using System.Reflection;

//namespace Corporate_Banking_Payment_Application.Services
//{
//    public class ReportService : IReportService
//    {
//        private readonly IReportRepository _reportRepo;
//        private readonly IUserRepository _userRepo;
//        private readonly IPaymentRepository _paymentRepo;
//        private readonly ISalaryDisbursementRepository _salaryRepo;
//        private readonly IClientRepository _clientRepo;
//        private readonly IMapper _mapper;
//        private readonly Cloudinary _cloudinary;
//        private readonly ILogger<ReportService> _logger;

//        public ReportService(
//            IReportRepository reportRepo,
//            IUserRepository userRepo,
//            IPaymentRepository paymentRepo,
//            ISalaryDisbursementRepository salaryRepo,
//            IClientRepository clientRepo,
//            IMapper mapper,
//            Cloudinary cloudinary,
//            ILogger<ReportService> logger)
//        {
//            _reportRepo = reportRepo;
//            _userRepo = userRepo;
//            _paymentRepo = paymentRepo;
//            _salaryRepo = salaryRepo;
//            _clientRepo = clientRepo;
//            _mapper = mapper;
//            _cloudinary = cloudinary;
//            _logger = logger;
//        }

//        // --- Core Business Logic (Implementing IReportService methods) ---

//        public async Task<ReportDto> GenerateAndSaveReport(GenerateReportRequestDto request, int userId)
//        {
//            // 1. Validate and Authorize Request
//            var user = await ValidateAndAuthorizeRequest(request, userId);

//            // 2. Fetch Report Data
//            (IEnumerable<object> data, ReportType reportType) = await GetReportData(request, user);

//            // 3. Generate File Content
//            using var fileStream = ReportGenerator.Generate(data, request.OutputFormat, reportType);

//            // 4. Upload to Cloudinary
//            // Note: ReportName is used here for file naming, not the final saved ReportName property
//            var uploadResult = await UploadReportToCloudinary(fileStream, request.ReportName, request.OutputFormat);

//            // 5. Save Report Record
//            // FIX: Create the Report Model directly and map from the request DTO
//            var reportToCreate = new Report
//            {
//                ReportName = request.ReportName,
//                ReportType = request.ReportType, // Use the type from the request
//                GeneratedBy = userId,
//                OutputFormat = request.OutputFormat,
//                FilePath = uploadResult.SecureUrl.ToString()
//            };

//            var created = await _reportRepo.AddReport(reportToCreate);

//            return _mapper.Map<ReportDto>(created);
//        }

//        public async Task<ReportDto?> GetReportById(int reportId)
//        {
//            var report = await _reportRepo.GetReportById(reportId);
//            return report == null ? null : _mapper.Map<ReportDto>(report);
//        }

//        public async Task<IEnumerable<ReportDto>> GetReportsByUserId(int userId)
//        {
//            var reports = await _reportRepo.GetReportsByUserId(userId);
//            return _mapper.Map<IEnumerable<ReportDto>>(reports);
//        }

//        // --- Internal Helper Methods ---

//        private async Task<User> ValidateAndAuthorizeRequest(GenerateReportRequestDto request, int userId)
//        {
//            var user = await _userRepo.GetUserById(userId)
//                ?? throw new Exception("Generating user not found.");

//            if (request.ClientId.HasValue)
//            {
//                var client = await _clientRepo.GetClientById(request.ClientId.Value);
//                if (client == null)
//                    throw new Exception($"Client ID {request.ClientId.Value} specified in request is invalid.");
//            }

//            switch (user.UserRole)
//            {
//                case UserRole.SUPERADMIN:
//                    break;

//                case UserRole.BANKUSER:
//                    break;

//                case UserRole.CLIENTUSER:
//                    var customerId = user.Customer?.CustomerId ?? throw new Exception("Client User not linked to a Customer account.");

//                    var clientUser = await _clientRepo.GetClientByCustomerId(customerId)
//                        ?? throw new Exception("Client User's Customer not linked to a Client account.");

//                    if (request.ClientId.HasValue && request.ClientId.Value != clientUser.ClientId)
//                    {
//                        throw new UnauthorizedAccessException("Client User cannot generate reports for other clients.");
//                    }
//                    request.ClientId = clientUser.ClientId;
//                    break;

//                default:
//                    throw new UnauthorizedAccessException("User role not authorized for report generation.");
//            }

//            return user;
//        }

//        private async Task<(IEnumerable<object> data, ReportType reportType)> GetReportData(GenerateReportRequestDto request, User user)
//        {
//            switch (request.ReportType)
//            {
//                case ReportType.PAYMENT:
//                    var payments = await _paymentRepo.GetPaymentsByClientId(request.ClientId.Value);
//                    var filteredPayments = ApplyDateRangeFilter(payments, request.StartDate, request.EndDate);
//                    return (filteredPayments, ReportType.PAYMENT);

//                case ReportType.SALARY:
//                    var salaries = await _salaryRepo.GetByClientId(request.ClientId.Value);
//                    var filteredSalaries = ApplyDateRangeFilter(salaries, request.StartDate, request.EndDate);
//                    return (filteredSalaries, ReportType.SALARY);

//                case ReportType.TRANSACTION:
//                    var allPayments = await _paymentRepo.GetPaymentsByClientId(request.ClientId.Value);
//                    var allSalaries = await _salaryRepo.GetByClientId(request.ClientId.Value);

//                    var combinedTransactions = new List<object>();
//                    combinedTransactions.AddRange(allPayments);
//                    combinedTransactions.AddRange(allSalaries);

//                    var filteredTransactions = ApplyDateRangeFilter(combinedTransactions, request.StartDate, request.EndDate);

//                    return (filteredTransactions, ReportType.TRANSACTION);

//                default:
//                    throw new ArgumentException($"Unsupported ReportType: {request.ReportType}");
//            }
//        }

//        private async Task<RawUploadResult> UploadReportToCloudinary(Stream fileStream, string reportName, ReportOutputFormat format)
//        {
//            string fileExtension = format == ReportOutputFormat.EXCEL ? "xlsx" : "pdf";
//            string publicId = $"reports/{reportName.Replace(" ", "-")}-{DateTime.UtcNow.Ticks}";

//            var uploadParams = new RawUploadParams()
//            {
//                File = new FileDescription($"{publicId}.{fileExtension}", fileStream),
//                PublicId = publicId,
//                Folder = "corporate_banking_reports",
//                Type = "authenticated"
//            };

//            RawUploadResult uploadResult;
//            try
//            {
//                uploadResult = await _cloudinary.UploadAsync(uploadParams);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to upload report to Cloudinary for report: {ReportName}", reportName);
//                throw new Exception("Report file upload failed due to external service error.", ex);
//            }

//            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
//            {
//                _logger.LogError("Cloudinary upload failed with status: {Status} and message: {Message}",
//                    uploadResult.StatusCode, uploadResult.Error?.Message);
//                throw new Exception($"Cloudinary report upload failed: {uploadResult.Error?.Message}");
//            }

//            return uploadResult;
//        }

//        private IEnumerable<T> ApplyDateRangeFilter<T>(IEnumerable<T> data, DateTime? startDate, DateTime? endDate)
//        {
//            if (!startDate.HasValue && !endDate.HasValue)
//                return data;

//            var dateProperty = typeof(T).GetProperty("RequestDate");

//            if (dateProperty == null)
//            {
//                dateProperty = typeof(T).GetProperty("Date");
//            }

//            if (dateProperty == null || (dateProperty.PropertyType != typeof(DateTime) && dateProperty.PropertyType != typeof(DateTime?)))
//            {
//                return data;
//            }

//            return data.Where(item =>
//            {
//                var dateValue = (DateTime?)dateProperty.GetValue(item);
//                if (!dateValue.HasValue) return false;

//                bool afterStart = !startDate.HasValue || dateValue.Value.Date >= startDate.Value.Date;
//                bool beforeEnd = !endDate.HasValue || dateValue.Value.Date <= endDate.Value.Date;

//                return afterStart && beforeEnd;
//            });
//        }
//    }
//}

using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
// FIX: Removed duplicate 'using' statements
using corporate_banking_payment_application.DTOs;
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

        // --- Core Business Logic (Implementing IReportService methods) ---

        // FIX: Signature now matches IReportService (added UserRole parameter)
        //public async Task<ReportDto> GenerateAndSaveReport(GenerateReportRequestDto request, int currentUserId, UserRole currentUserRole)
        //{
        //    // 1. Validate and Authorize Request
        //    // Pass the role to the validation method
        //    var user = await ValidateAndAuthorizeRequest(request, currentUserId, currentUserRole);

        //    // 2. Fetch Report Data
        //    (IEnumerable<object> data, ReportType reportType) = await GetReportData(request, user);

        //    // 3. Generate File Content
        //    using var fileStream = ReportGenerator.Generate(data, request.OutputFormat, reportType);

        //    // 4. Upload to Cloudinary
        //    var uploadResult = await UploadReportToCloudinary(fileStream, request.ReportName, request.OutputFormat);

        //    // 5. Save Report Record
        //    var reportToCreate = new Report
        //    {
        //        ReportName = request.ReportName,
        //        ReportType = request.ReportType,
        //        GeneratedBy = currentUserId, // Use the parameter
        //        OutputFormat = request.OutputFormat,
        //        FilePath = uploadResult.SecureUrl.ToString()
        //    };

        //    var created = await _reportRepo.AddReport(reportToCreate);

        //    return _mapper.Map<ReportDto>(created);
        //}

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

                        // Call generator with the specific type: Payment
                        fileStream = ReportGenerator.Generate(filteredPayments, request.OutputFormat, request.ReportType);
                        break;
                    }

                case ReportType.SALARY:
                    {
                        var salaries = await _salaryRepo.GetByClientId(clientId);
                        var filteredSalaries = ApplyDateRangeFilter(salaries, request.StartDate, request.EndDate);

                        // Call generator with the specific type: SalaryDisbursement
                        fileStream = ReportGenerator.Generate(filteredSalaries, request.OutputFormat, request.ReportType);
                        break;
                    }

                case ReportType.TRANSACTION:
                    {
                        // For TRANSACTION, we use our new TransactionReportDto
                        var allPayments = await _paymentRepo.GetPaymentsByClientId(clientId);
                        var allSalaries = await _salaryRepo.GetByClientId(clientId);

                        // Map both lists to the common DTO
                        var combinedTransactions = new List<TransactionReportDto>();
                        combinedTransactions.AddRange(allPayments.Select(p => new TransactionReportDto { Date = p.RequestDate, Amount = p.Amount, Type = "Payment", Description = p.Description }));
                        combinedTransactions.AddRange(allSalaries.Select(s => new TransactionReportDto { Date = s.Date, Amount = s.Amount, Type = "Salary", Description = s.Description }));

                        var filteredTransactions = ApplyDateRangeFilter(combinedTransactions, request.StartDate, request.EndDate);

                        // Call generator with the specific type: TransactionReportDto
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

            // 4. Reset the stream's position for the Cloudinary upload
            //fileStream.Position = 0;




            //// 3. Upload and Save (This logic is wrapped in a 'using' block)
            //using (fileStream) // Ensures the stream is disposed after upload
            //{
            //    // 4. Upload to Cloudinary
            //    var uploadResult = await UploadReportToCloudinary(fileStream, request.ReportName, request.OutputFormat);

            //    // 5. Save Report Record
            //    var reportToCreate = new Report
            //    {
            //        ReportName = request.ReportName,
            //        ReportType = request.ReportType,
            //        GeneratedBy = currentUserId,
            //        OutputFormat = request.OutputFormat,
            //        FilePath = uploadResult.SecureUrl.ToString()
            //    };

            //    var created = await _reportRepo.AddReport(reportToCreate);
            //    return _mapper.Map<ReportDto>(created);
            //}


            // 5. Upload and Save
            // ⭐️ FIX: Create a NEW, clean stream from the byte array for Cloudinary
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

        // FIX: Method name now matches IReportService
        public async Task<IEnumerable<ReportDto>> GetReportsByUser(int userId)
        {
            // The repository call can remain the same
            var reports = await _reportRepo.GetReportsByUserId(userId);
            return _mapper.Map<IEnumerable<ReportDto>>(reports);
        }

        // --- Internal Helper Methods ---

        // FIX: Signature updated to accept UserRole
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

        //private async Task<(IEnumerable<object> data, ReportType reportType)> GetReportData(GenerateReportRequestDto request, User user)
        //{
        //    // Ensure ClientId is available for non-SuperAdmin roles if required by the report type
        //    if (user.UserRole != UserRole.SUPERADMIN && !request.ClientId.HasValue)
        //    {
        //        // For ClientUser, ClientId is set automatically in ValidateAndAuthorizeRequest
        //        // For BankUser, they might need to specify one.
        //        if (user.UserRole == UserRole.BANKUSER && !request.ClientId.HasValue)
        //        {
        //            throw new ArgumentException("ClientId is required for BankUser to generate this report.");
        //        }
        //        else if (user.UserRole == UserRole.CLIENTUSER && !request.ClientId.HasValue)
        //        {
        //            // This should not be hit if validation logic is correct, but as a safeguard:
        //            throw new ArgumentException("ClientId could not be determined for ClientUser.");
        //        }
        //    }

        //    // Use ClientId only if it's set (SuperAdmin might request a non-client-specific report,
        //    // or BankUser might request a bank-wide one, depending on requirements)
        //    // For now, we'll assume ClientId is required for all data-fetching.
        //    if (!request.ClientId.HasValue)
        //    {
        //        throw new ArgumentException("A ClientId must be provided for this report type.");
        //    }

        //    var clientId = request.ClientId.Value;

        //    switch (request.ReportType)
        //    {
        //        case ReportType.PAYMENT:
        //            var payments = await _paymentRepo.GetPaymentsByClientId(clientId);
        //            var filteredPayments = ApplyDateRangeFilter(payments, request.StartDate, request.EndDate);
        //            return (filteredPayments, ReportType.PAYMENT);

        //        case ReportType.SALARY:
        //            var salaries = await _salaryRepo.GetByClientId(clientId);
        //            var filteredSalaries = ApplyDateRangeFilter(salaries, request.StartDate, request.EndDate);
        //            return (filteredSalaries, ReportType.SALARY);

        //        case ReportType.TRANSACTION:
        //            var allPayments = await _paymentRepo.GetPaymentsByClientId(clientId);
        //            var allSalaries = await _salaryRepo.GetByClientId(clientId);

        //            var combinedTransactions = new List<object>();
        //            combinedTransactions.AddRange(allPayments);
        //            combinedTransactions.AddRange(allSalaries);

        //            var filteredTransactions = ApplyDateRangeFilter(combinedTransactions, request.StartDate, request.EndDate);

        //            return (filteredTransactions, ReportType.TRANSACTION);

        //        default:
        //            throw new ArgumentException($"Unsupported ReportType: {request.ReportType}");
        //    }
        //}

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
