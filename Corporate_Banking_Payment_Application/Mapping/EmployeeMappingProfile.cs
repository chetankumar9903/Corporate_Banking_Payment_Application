using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {

            CreateMap<Employee, EmployeeDto>();


            CreateMap<CreateEmployeeDto, Employee>();


            CreateMap<UpdateEmployeeDto, Employee>()

                .ForMember(dest => dest.Salary,
                           opt => opt.Condition(src => src.Salary.HasValue))

                .ForMember(dest => dest.Balance,
                           opt => opt.Condition(src => src.Balance.HasValue))

                .ForMember(dest => dest.IsActive,
                           opt => opt.Condition(src => src.IsActive.HasValue));

        }
    }
}
