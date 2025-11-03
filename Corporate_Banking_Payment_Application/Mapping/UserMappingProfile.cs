using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {

            // User -> UserDto
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.UserRole,
                           opt => opt.MapFrom(src => src.UserRole.ToString())); // Enum to string

            // CreateUserDto -> User (hash password)
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Password,
                           opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)))
                .ForMember(dest => dest.UserRole,
                           opt => opt.MapFrom(src => src.UserRole));

            // UpdateUserDto -> User
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.UserRole,
                           opt => opt.MapFrom(src => src.UserRole));
        }
    }
}

