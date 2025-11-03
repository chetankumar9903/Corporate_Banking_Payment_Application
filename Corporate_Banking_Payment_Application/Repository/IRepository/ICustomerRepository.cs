using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface ICustomerRepository
    {
        //Task<IEnumerable<Customer>> GetAllCustomers();
        Task<PagedResult<Customer>> GetAllCustomers(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<Customer?> GetCustomerById(int id);
        Task<Customer> AddCustomer(Customer customer);
        Task<Customer> UpdateCustomer(Customer customer);
        Task<bool> DeleteCustomer(int id);

        Task<Customer?> GetCustomerByUserId(int userId);

        //Task<bool> ExistsCustomer(int id);
    }
}
