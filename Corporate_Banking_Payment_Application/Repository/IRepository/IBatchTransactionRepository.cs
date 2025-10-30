using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface IBatchTransactionRepository
    {
        Task<IEnumerable<BatchTransaction>> GetAll();
        Task<BatchTransaction?> GetById(int id);
        Task<IEnumerable<BatchTransaction>> GetByClientId(int clientId);
        Task<BatchTransaction> Add(BatchTransaction entity);
        Task Update(BatchTransaction batch);
        Task Delete(BatchTransaction batch);
    }
}
