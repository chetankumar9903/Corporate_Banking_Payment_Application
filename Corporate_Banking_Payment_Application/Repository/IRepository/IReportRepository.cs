using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface IReportRepository
    {
        Task<Report> AddReport(Report report);

        Task<PagedResult<Report>> GetReportsByUserId(int userId, string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<Report?> GetReportById(int id);
    }
}
