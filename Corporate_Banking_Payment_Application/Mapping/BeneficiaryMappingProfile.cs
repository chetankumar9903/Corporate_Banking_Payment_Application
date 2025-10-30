using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class BeneficiaryMappingProfile : Profile
    {
        public BeneficiaryMappingProfile()
        {
            // Entity → DTO
            CreateMap<Beneficiary, BeneficiaryDto>();

            // DTO → Entity
            CreateMap<CreateBeneficiaryDto, Beneficiary>();

            // Update DTO → Entity (ignore nulls)
            CreateMap<UpdateBeneficiaryDto, Beneficiary>()
                .ForAllMembers(opt => opt.Condition(
                    (src, dest, srcMember) => srcMember != null
                ));
        }
    }
}
