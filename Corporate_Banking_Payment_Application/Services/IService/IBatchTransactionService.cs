using Corporate_Banking_Payment_Application.DTOs;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IBatchTransactionService
    {
        Task<IEnumerable<BatchTransactionDto>> GetAllBatches();
        Task<BatchTransactionDto?> GetBatchById(int id);
        Task<IEnumerable<BatchTransactionDto>> GetByClientId(int clientId);
        //Task<BatchTransactionDto> CreateBatch(CreateBatchTransactionDto dto);
        Task<object> CreateBatch(CreateBatchTransactionDto dto);
        Task<bool> DeleteBatch(int id);

        Task<object> ProcessBatchCsv(IFormFile file, int clientId);

        Task<bool> HasReceivedSalaryInLast30Days(int employeeId);
    }
}
