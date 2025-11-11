using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IEmployeeService
    {

        //Task<IEnumerable<EmployeeDto>> GetAllEmployees();
        Task<PagedResult<EmployeeDto>> GetAllEmployees(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<EmployeeDto?> GetEmployeeById(int id);
        Task<EmployeeDto> CreateEmployee(CreateEmployeeDto dto);
        Task<EmployeeDto?> UpdateEmployee(int id, UpdateEmployeeDto dto);
        Task<bool> DeleteEmployee(int id);

        Task<IEnumerable<EmployeeDto>> GetEmployeesByClientId(int clientId);

        Task<object> ProcessEmployeeCsv(IFormFile file, int clientId);
    }
}
