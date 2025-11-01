using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface ISalaryDisbursementService
    {
        //Task<IEnumerable<SalaryDisbursementDto>> GetAll();
        Task<PagedResult<SalaryDisbursementDto>> GetAll(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<SalaryDisbursementDto?> GetById(int id);
        Task<IEnumerable<SalaryDisbursementDto>> GetByClientId(int clientId);
        Task<IEnumerable<SalaryDisbursementDto>> GetByEmployeeId(int employeeId);
        Task<SalaryDisbursementDto> Create(CreateSalaryDisbursementDto dto);
        Task<SalaryDisbursementDto?> Update(int id, UpdateSalaryDisbursementDto dto);
        Task<bool> Delete(int id);
    }
}
