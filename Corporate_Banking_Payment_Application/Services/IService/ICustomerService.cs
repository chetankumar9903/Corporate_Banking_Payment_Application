using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomers();
        Task<CustomerDto?> GetCustomerById(int id);
        Task<CustomerDto> CreateCustomer(CreateCustomerDto dto);
        Task<CustomerDto> UpdateCustomer(int id, UpdateCustomerDto dto);
        Task<bool> DeleteCustomer(int id);

        Task<CustomerDto> UpdateStatus(int id, Status newStatus);
    }
}
