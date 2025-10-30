using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class BankMappingProfile : Profile
    {
        public BankMappingProfile()
        {

            CreateMap<Bank, BankDto>();

            CreateMap<CreateBankDto, Bank>();
            CreateMap<UpdateBankDto, Bank>();
        }

    }
}
