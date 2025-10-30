using Corporate_Banking_Payment_Application.DTOs;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IEmployeeService
    {
        // CRUD Operations returning DTOs
        Task<IEnumerable<EmployeeDto>> GetAllEmployees();
        Task<EmployeeDto?> GetEmployeeById(int id);
        Task<EmployeeDto> CreateEmployee(CreateEmployeeDto dto);
        Task<EmployeeDto?> UpdateEmployee(int id, UpdateEmployeeDto dto);
        Task<bool> DeleteEmployee(int id);

        Task<IEnumerable<EmployeeDto>> GetEmployeesByClientId(int clientId);
    }
}
