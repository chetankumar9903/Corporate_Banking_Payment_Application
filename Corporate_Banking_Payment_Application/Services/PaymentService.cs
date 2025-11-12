using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;

namespace Corporate_Banking_Payment_Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IBeneficiaryRepository _beneficiaryRepo;
        private readonly IMapper _mapper;


        public PaymentService(
            IPaymentRepository paymentRepo,
            IClientRepository clientRepo,
            IBeneficiaryRepository beneficiaryRepo,
            IMapper mapper)
        {
            _paymentRepo = paymentRepo;
            _clientRepo = clientRepo;
            _beneficiaryRepo = beneficiaryRepo;
            _mapper = mapper;
        }

        //public async Task<IEnumerable<PaymentDto>> GetAllPayments()
        //{
        //    var payments = await _paymentRepo.GetAllPayments();
        //    return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        //}

        public async Task<PagedResult<PaymentDto>> GetAllPayments(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            var pagedResult = await _paymentRepo.GetAllPayments(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            var itemsDto = _mapper.Map<IEnumerable<PaymentDto>>(pagedResult.Items);
            return new PagedResult<PaymentDto>
            {
                Items = itemsDto.ToList(),
                TotalCount = pagedResult.TotalCount
            };
        }

        public async Task<PaymentDto?> GetPaymentById(int id)
        {
            var payment = await _paymentRepo.GetPaymentById(id);
            return payment == null ? null : _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> CreatePayment(CreatePaymentDto dto)
        {

            var client = await _clientRepo.GetClientById(dto.ClientId)
                ?? throw new Exception($"Client with ID {dto.ClientId} not found.");

            if (!client.IsActive)
                throw new Exception("Client account is inactive. Cannot initiate payment.");


            var beneficiary = await _beneficiaryRepo.GetBeneficiaryById(dto.BeneficiaryId)
                ?? throw new Exception($"Beneficiary with ID {dto.BeneficiaryId} not found.");

            if (!beneficiary.IsActive)
                throw new Exception("Beneficiary is inactive. Cannot make payment.");


            if (beneficiary.ClientId != dto.ClientId)
                throw new Exception("Unauthorized: Beneficiary does not belong to this client.");


            if (client.Balance < dto.Amount)
                throw new Exception("Insufficient balance to process this payment request.");


            var existingPayments = await _paymentRepo.GetPaymentsByClientId(dto.ClientId);
            if (existingPayments.Any(p => p.BeneficiaryId == dto.BeneficiaryId &&
                                          p.Amount == dto.Amount &&
                                          p.PaymentStatus == Status.PENDING))
            {
                throw new Exception("Duplicate pending payment detected for the same beneficiary and amount.");
            }


            var payment = _mapper.Map<Payment>(dto);
            payment.RequestDate = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
            );
            payment.PaymentStatus = Status.PENDING;

            var created = await _paymentRepo.AddPayment(payment);

            return _mapper.Map<PaymentDto>(created);
        }

        public async Task<PaymentDto?> UpdatePayment(int id, UpdatePaymentDto dto)
        {

            var existing = await _paymentRepo.GetPaymentById(id)
       ?? throw new Exception($"Payment with ID {id} not found.");


            if (existing.PaymentStatus != Status.PENDING)
                throw new Exception("Only pending payments can be updated.");


            if (!dto.PaymentStatus.HasValue)
                throw new Exception("Payment status update is required.");

            var newStatus = dto.PaymentStatus.Value;


            if (newStatus == Status.APPROVED)
            {
                var client = await _clientRepo.GetClientById(existing.ClientId)
                    ?? throw new Exception("Client not found for this payment.");


                if (client.Balance < existing.Amount)
                    throw new Exception("Insufficient balance to approve payment.");

                client.Balance -= existing.Amount;
                await _clientRepo.UpdateClient(client);

                existing.PaymentStatus = Status.APPROVED;
                existing.ProcessedDate = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );
            }
            else if (newStatus == Status.REJECTED)
            {
                existing.PaymentStatus = Status.REJECTED;
                existing.RejectReason = dto.RejectReason ?? "No reason specified.";
                existing.ProcessedDate = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );
            }
            else
            {
                throw new Exception("Invalid status update. Only APPROVED or REJECTED allowed.");
            }

            await _paymentRepo.UpdatePayment(existing);
            return _mapper.Map<PaymentDto>(existing);
        }

        public async Task<bool> DeletePayment(int id)
        {
            var payment = await _paymentRepo.GetPaymentById(id);
            if (payment == null) return false;


            if (payment.PaymentStatus != Status.PENDING)
            {

                return false;
            }

            await _paymentRepo.DeletePayment(payment);
            return true;
        }


        public async Task<IEnumerable<PaymentDto>> GetPaymentsByClientId(int clientId)
        {
            var payments = await _paymentRepo.GetPaymentsByClientId(clientId);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByBeneficiaryId(int beneficiaryId)
        {
            var payments = await _paymentRepo.GetPaymentsByBeneficiaryId(beneficiaryId);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByStatus(Status status)
        {
            var payments = await _paymentRepo.GetPaymentsByStatus(status);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }



    }
}
