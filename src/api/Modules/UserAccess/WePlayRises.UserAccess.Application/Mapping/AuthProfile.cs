using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WePlayRises.UserAccess.Application.Dtos;

namespace WePlayRises.UserAccess.Application.Mapping;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        // IdentityUser -> RegisterResponseDto
        CreateMap<IdentityUser, RegisterResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        // IdentityUser -> LoginResponseDto
        CreateMap<IdentityUser, LoginResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        // IdentityUser -> UserInfoDto
        CreateMap<IdentityUser, UserInfoDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
            .ForMember(dest => dest.Roles, opt => opt.Ignore());
    }
}
