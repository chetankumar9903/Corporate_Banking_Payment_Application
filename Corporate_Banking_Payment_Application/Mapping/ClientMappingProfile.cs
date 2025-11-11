using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class ClientMappingProfile : Profile
    {

        public ClientMappingProfile()
        {

            CreateMap<Client, ClientDto>()
                .ForMember(dest => dest.CustomerName,
                           opt => opt.MapFrom(src => src.Customer != null ? src.Customer.User.UserName : null))
                .ForMember(dest => dest.BankName,
                           opt => opt.MapFrom(src => src.Bank != null ? src.Bank.BankName : null));


            CreateMap<CreateClientDto, Client>()
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.AccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.InitialBalance))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateClientDto, Client>();
        }
    }
}
