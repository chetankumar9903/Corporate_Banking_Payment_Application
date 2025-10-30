using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            // Employee -> EmployeeDto
            CreateMap<Employee, EmployeeDto>();

            // CreateEmployeeDto -> Employee
            CreateMap<CreateEmployeeDto, Employee>();

            // UpdateEmployeeDto -> Employee (Revised)
            CreateMap<UpdateEmployeeDto, Employee>()
                // Salary update: only map if the DTO value is not null
                .ForMember(dest => dest.Salary,
                           opt => opt.Condition(src => src.Salary.HasValue))
                // Balance update: only map if the DTO value is not null
                .ForMember(dest => dest.Balance,
                           opt => opt.Condition(src => src.Balance.HasValue))
                // IsActive update: only map if the DTO value is not null
                .ForMember(dest => dest.IsActive,
                           opt => opt.Condition(src => src.IsActive.HasValue));

            // Properties like JoinDate and AccountNumber are now automatically ignored 
            // because they don't exist in the UpdateEmployeeDto.
        }
    }
}
