using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class BeneficiaryMappingProfile : Profile
    {
        public BeneficiaryMappingProfile()
        {

            CreateMap<Beneficiary, BeneficiaryDto>();


            CreateMap<CreateBeneficiaryDto, Beneficiary>();

            CreateMap<UpdateBeneficiaryDto, Beneficiary>()
                .ForAllMembers(opt => opt.Condition(
                    (src, dest, srcMember) => srcMember != null
                ));
        }
    }
}
