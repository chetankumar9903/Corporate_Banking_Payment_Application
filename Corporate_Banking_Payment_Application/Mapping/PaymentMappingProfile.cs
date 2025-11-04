//using AutoMapper;
//using Corporate_Banking_Payment_Application.DTOs;
//using Corporate_Banking_Payment_Application.Models;

//namespace Corporate_Banking_Payment_Application.Mapping
//{
//    public class PaymentMappingProfile : Profile
//    {
//        public PaymentMappingProfile()
//        {
//            // 1. Payment (Model) -> PaymentDto (Read Operation)
//            // Simple mapping is sufficient as property names match
//            CreateMap<Payment, PaymentDto>();

//            // 2. CreatePaymentDto -> Payment (Write Operation)
//            // Maps the required DTO fields to the model.
//            // RequestDate and initial PaymentStatus will be handled by the model defaults
//            // or explicitly by the service layer after mapping.
//            CreateMap<CreatePaymentDto, Payment>();

//            // 3. UpdatePaymentDto -> Payment (Update Status Operation)
//            // This mapping handles partial updates, focusing only on status and reason.
//            CreateMap<UpdatePaymentDto, Payment>()
//                // PaymentStatus is technically required in the DTO, but we use Condition 
//                // for best practice on value types to prevent accidental overwrites if used in a PATCH context.
//                .ForMember(dest => dest.PaymentStatus,
//                           opt => opt.Condition(src => src.PaymentStatus.HasValue))

//                // RejectReason is a nullable string, so we ensure it's mapped if provided.
//                // It will overwrite the destination if null is explicitly passed, 
//                // which is acceptable if the status is changing from REJECTED back to PENDING/APPROVED (though rare).
//                .ForMember(dest => dest.RejectReason,
//                           opt => opt.MapFrom(src => src.RejectReason));
//        }
//    }
//}
using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {
            // --- ADD THESE MAPPINGS ---

            // Maps the nested Client entity to the new NestedClientDto
            CreateMap<Client, NestedClientDto>();

            // Maps the nested Beneficiary entity to the new NestedBeneficiaryDto
            CreateMap<Beneficiary, NestedBeneficiaryDto>();

            // --- UPDATE THIS MAPPING ---

            // 1. Payment (Model) -> PaymentDto (Read Operation)
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
                .ForMember(dest => dest.Beneficiary, opt => opt.MapFrom(src => src.Beneficiary));

            // --- END OF UPDATES ---


            // 2. CreatePaymentDto -> Payment (Write Operation)
            CreateMap<CreatePaymentDto, Payment>();

            // 3. UpdatePaymentDto -> Payment (Update Status Operation)
            CreateMap<UpdatePaymentDto, Payment>()
        .ForMember(dest => dest.PaymentStatus,
             opt => opt.Condition(src => src.PaymentStatus.HasValue))
        .ForMember(dest => dest.RejectReason,
             opt => opt.MapFrom(src => src.RejectReason));
        }
    }
}