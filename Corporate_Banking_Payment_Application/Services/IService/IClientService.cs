using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IClientService
    {

        //Task<IEnumerable<ClientDto>> GetAllClients();
        Task<PagedResult<ClientDto>> GetAllClients(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);


        Task<ClientDto?> GetClientById(int id);


        Task<ClientDto> CreateClient(CreateClientDto dto);


        Task<ClientDto> UpdateClient(int id, UpdateClientDto dto);


        Task<bool> DeleteClient(int id);

        Task<ClientDto?> GetClientByCustomerId(int customerId);

        Task<ClientDto> UpdateClientBalance(int clientId, UpdateClientBalanceDto dto);
    }
}
