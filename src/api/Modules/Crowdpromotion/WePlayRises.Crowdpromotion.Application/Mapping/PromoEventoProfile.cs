using AutoMapper;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Commands;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Mapping;

public class PromoEventoProfile : Profile
{
    public PromoEventoProfile()
    {
        // RegistrarEventoCommand -> PromoEvento
        CreateMap<RegistrarEventoCommand, PromoEvento>()
            .ForMember(dest => dest.TipoEventoId, opt => opt.MapFrom(src => src.TipoEventoPromoId))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProgramaId, opt => opt.Ignore())
            .ForMember(dest => dest.PromotorId, opt => opt.Ignore())
            .ForMember(dest => dest.PromoProgramaPromotorId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaId, opt => opt.Ignore())
            .ForMember(dest => dest.ImporteAsociado, opt => opt.Ignore())
            .ForMember(dest => dest.AportacionCrowdfundingId, opt => opt.Ignore())
            .ForMember(dest => dest.PedidoCrowdfundingId, opt => opt.Ignore())
            .ForMember(dest => dest.Programa, opt => opt.Ignore())
            .ForMember(dest => dest.Promotor, opt => opt.Ignore());

        // RegistrarConversionCommand -> PromoEvento
        CreateMap<RegistrarConversionCommand, PromoEvento>()
            .ForMember(dest => dest.ImporteAsociado, opt => opt.MapFrom(src => src.ValorMonetario))
            .ForMember(dest => dest.TipoEventoId, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProgramaId, opt => opt.Ignore())
            .ForMember(dest => dest.PromotorId, opt => opt.Ignore())
            .ForMember(dest => dest.PromoProgramaPromotorId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.IpOrigen, opt => opt.Ignore())
            .ForMember(dest => dest.UserAgentOrigen, opt => opt.Ignore())
            .ForMember(dest => dest.UrlOrigen, opt => opt.Ignore())
            .ForMember(dest => dest.UrlReferer, opt => opt.Ignore())
            .ForMember(dest => dest.UtmSource, opt => opt.Ignore())
            .ForMember(dest => dest.UtmMedium, opt => opt.Ignore())
            .ForMember(dest => dest.UtmCampaign, opt => opt.Ignore())
            .ForMember(dest => dest.PedidoCrowdfundingId, opt => opt.Ignore())
            .ForMember(dest => dest.Programa, opt => opt.Ignore())
            .ForMember(dest => dest.Promotor, opt => opt.Ignore());
    }
}
