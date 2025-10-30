using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllClients();
        Task<Client?> GetClientById(int id);
        Task<Client?> GetClientByCustomerId(int customerId);
        Task<Client> AddClient(Client client);
        Task<Client> UpdateClient(Client client);
        Task<bool> DeleteClient(int id);
    }
}
