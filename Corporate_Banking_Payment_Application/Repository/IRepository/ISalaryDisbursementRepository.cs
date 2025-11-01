using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface ISalaryDisbursementRepository
    {
        //Task<IEnumerable<SalaryDisbursement>> GetAll();
        Task<PagedResult<SalaryDisbursement>> GetAll(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<SalaryDisbursement?> GetById(int id);
        Task<IEnumerable<SalaryDisbursement>> GetByClientId(int clientId);
        Task<IEnumerable<SalaryDisbursement>> GetByEmployeeId(int employeeId);
        Task<SalaryDisbursement> Add(SalaryDisbursement entity);
        //Task<SalaryDisbursement> Update(SalaryDisbursement entity);
        Task Update(SalaryDisbursement entity);
        //Task<bool> Delete(int id);
        Task Delete(SalaryDisbursement entity);
    }
}
