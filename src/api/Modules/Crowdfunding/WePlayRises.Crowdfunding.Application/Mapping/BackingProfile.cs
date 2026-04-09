using AutoMapper;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Mapping;

public class BackingProfile : Profile
{
    public BackingProfile()
    {
        // PedidoCrowdfunding -> BackingDto
        CreateMap<PedidoCrowdfunding, BackingDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.CampaniaId, opt => opt.MapFrom(src => src.CampaniaId.Value))
            .ForMember(dest => dest.Monto, opt => opt.MapFrom(src => src.ImporteTotal))
            .ForMember(dest => dest.Mensaje, opt => opt.MapFrom(src => src.ComentarioBacker))
            .ForMember(dest => dest.EsAnonimo, opt => opt.MapFrom(src => !src.PermitirMostrarNombre))
            .ForMember(dest => dest.RewardId, opt => opt.MapFrom(src =>
                src.Lineas.Any(l => l.EsRewardPrincipal)
                    ? src.Lineas.First(l => l.EsRewardPrincipal).RewardId.Value
                    : (Guid?)null))
            .ForMember(dest => dest.CampaniaTitulo, opt => opt.Ignore())
            .ForMember(dest => dest.RewardNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaSimbolo, opt => opt.Ignore())
            .ForMember(dest => dest.EstadoPedido, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.Ignore());

        // PedidoCrowdfunding -> BackingPublicDto
        CreateMap<PedidoCrowdfunding, BackingPublicDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.Monto, opt => opt.MapFrom(src => src.ImporteTotal))
            .ForMember(dest => dest.Mensaje, opt => opt.MapFrom(src => src.ComentarioBacker))
            .ForMember(dest => dest.NombreBacker, opt => opt.Ignore())
            .ForMember(dest => dest.RewardNombre, opt => opt.Ignore());
    }
}
