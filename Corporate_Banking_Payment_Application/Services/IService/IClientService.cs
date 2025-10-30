using Corporate_Banking_Payment_Application.DTOs;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IClientService
    {

        Task<IEnumerable<ClientDto>> GetAllClients();


        Task<ClientDto?> GetClientById(int id);


        Task<ClientDto> CreateClient(CreateClientDto dto);


        Task<ClientDto> UpdateClient(int id, UpdateClientDto dto);


        Task<bool> DeleteClient(int id);

        Task<ClientDto?> GetClientByCustomerId(int customerId);
    }
}
