
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;


namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IReportService
    {
        Task<ReportDto> GenerateAndSaveReport(GenerateReportRequestDto request, int currentUserId, UserRole currentUserRole);
        Task<PagedResult<ReportDto>> GetReportsByUser(int userId, string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<ReportDto?> GetReportById(int reportId);
    }
}
