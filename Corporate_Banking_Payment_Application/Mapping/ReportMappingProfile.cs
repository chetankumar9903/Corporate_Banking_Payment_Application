
using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class ReportMappingProfile : Profile
    {
        public ReportMappingProfile()
        {
            // 1. Report (Model) -> ReportDto (Read Operation / API Response)
            // Maps all matching properties including the OutputFormat
            CreateMap<Report, ReportDto>();

            // 2. CreateReportDto (Internal DTO) -> Report (Model)
            // Maps data for persisting the report to the database.
            CreateMap<CreateReportDto, Report>()
                // Ignore GeneratedDate on mapping, as the Model sets its own default value
                // upon creation, which is the correct behavior.
                .ForMember(dest => dest.GeneratedDate, opt => opt.Ignore());
        }
    }
}
