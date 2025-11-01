using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class SalaryDisbursementMappingProfile : Profile
    {
        public SalaryDisbursementMappingProfile()
        {

            // Entity → DTO
            CreateMap<SalaryDisbursement, SalaryDisbursementDto>()
                .ForMember(dest => dest.EmployeeName, opt =>
                    opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : null))
                .ForMember(dest => dest.ClientCompanyName, opt =>
                    opt.MapFrom(src => src.Client != null ? src.Client.CompanyName : null));

            // DTO → Entity
            CreateMap<CreateSalaryDisbursementDto, SalaryDisbursement>();
            CreateMap<UpdateSalaryDisbursementDto, SalaryDisbursement>()
                .ForMember(dest => dest.Amount, opt => opt.Condition(src => src.Amount.HasValue));
        }
    }
}
