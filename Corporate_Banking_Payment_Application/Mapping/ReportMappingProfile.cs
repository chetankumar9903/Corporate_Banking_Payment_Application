
using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class ReportMappingProfile : Profile
    {
        public ReportMappingProfile()
        {

            CreateMap<Report, ReportDto>();


            CreateMap<CreateReportDto, Report>()

                .ForMember(dest => dest.GeneratedDate, opt => opt.Ignore());
        }
    }
}
