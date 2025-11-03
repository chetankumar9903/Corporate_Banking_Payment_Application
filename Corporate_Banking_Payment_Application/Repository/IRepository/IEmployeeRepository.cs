using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface IEmployeeRepository
    {
        // CRUD Operations
        //Task<IEnumerable<Employee>> GetAllEmployees();
        Task<PagedResult<Employee>> GetAllEmployees(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<Employee?> GetEmployeeById(int id);
        Task<Employee> AddEmployee(Employee employee);
        Task UpdateEmployee(Employee employee);
        Task DeleteEmployee(Employee employee);
        Task<IEnumerable<Employee>> GetEmployeesByClientId(int clientId);

    }
}
