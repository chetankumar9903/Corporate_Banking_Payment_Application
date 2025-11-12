
using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {

            CreateMap<Client, NestedClientDto>();


            CreateMap<Beneficiary, NestedBeneficiaryDto>();


            CreateMap<Payment, PaymentDto>()
                      .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
                      .ForMember(dest => dest.Beneficiary, opt => opt.MapFrom(src => src.Beneficiary));


            CreateMap<CreatePaymentDto, Payment>();


            CreateMap<UpdatePaymentDto, Payment>()
              .ForMember(dest => dest.PaymentStatus,
                   opt => opt.Condition(src => src.PaymentStatus.HasValue))
              .ForMember(dest => dest.RejectReason,
                   opt => opt.MapFrom(src => src.RejectReason));

            CreateMap<Payment, PaymentDto>();


            CreateMap<CreatePaymentDto, Payment>();


            CreateMap<UpdatePaymentDto, Payment>()

                .ForMember(dest => dest.PaymentStatus,
                           opt => opt.Condition(src => src.PaymentStatus.HasValue))


                .ForMember(dest => dest.RejectReason,
                           opt => opt.MapFrom(src => src.RejectReason));

        }
    }
}