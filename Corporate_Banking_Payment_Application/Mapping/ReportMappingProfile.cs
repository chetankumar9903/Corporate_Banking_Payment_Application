//using AutoMapper;
//using Corporate_Banking_Payment_Application.DTOs;
//using Corporate_Banking_Payment_Application.Models;

//namespace Corporate_Banking_Payment_Application.Mapping
//{
//    public class ReportMappingProfile : Profile
//    {
//        public ReportMappingProfile()
//        {
//            // 1. Report (Model) -> ReportDto (Read Operation / API Response)
//            // Maps all matching properties including the new OutputFormat
//            CreateMap<Report, ReportDto>();

//            // 2. CreateReportDto (Internal DTO used by the service) -> Report (Model)
//            // Maps the data needed to persist the generated report to the database,
//            // including ReportType, GeneratedBy, FilePath, and the new OutputFormat.
//            CreateMap<CreateReportDto, Report>()
//                // Set the GeneratedDate default on the model itself (as you have done)
//                .ForMember(dest => dest.GeneratedDate, opt => opt.Ignore());
//        }
//    }
//}
using AutoMapper;
// FIX: Use lowercase namespaces to match your other files (like ReportDtos.cs)
using corporate_banking_payment_application.DTOs;
using Corporate_Banking_Payment_Application.Models;

// Using a capitalized namespace for your mapping files is fine,
// as long as the 'using' statements above are correct.
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
