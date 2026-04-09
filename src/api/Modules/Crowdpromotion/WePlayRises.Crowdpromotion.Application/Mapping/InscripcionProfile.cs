using AutoMapper;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Mapping;

public class InscripcionProfile : Profile
{
    public InscripcionProfile()
    {
        // PromoProgramaPromotor -> InscripcionCreadaDto
        CreateMap<PromoProgramaPromotor, InscripcionCreadaDto>()
            .ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaInscripcion))
            .ForMember(dest => dest.ProgramaId, opt => opt.MapFrom(src => src.ProgramaId.Value))
            .ForMember(dest => dest.ProgramaTitulo, opt => opt.Ignore());

        // PromoProgramaPromotor -> InscripcionAprobadaDto
        CreateMap<PromoProgramaPromotor, InscripcionAprobadaDto>()
            .ForMember(dest => dest.UrlTrackingPersonalizada, opt => opt.MapFrom(src => src.UrlReferido))
            .ForMember(dest => dest.PromotorNombre, opt => opt.Ignore());

        // PromoProgramaPromotor -> InscripcionBloqueadaDto
        CreateMap<PromoProgramaPromotor, InscripcionBloqueadaDto>()
            .ForMember(dest => dest.PromotorNombre, opt => opt.Ignore());

        // PromoProgramaPromotor -> InscripcionDadaDeBajaDto
        CreateMap<PromoProgramaPromotor, InscripcionDadaDeBajaDto>()
            .ForMember(dest => dest.PromotorNombre, opt => opt.Ignore());

        // PromoProgramaPromotor -> InscripcionListItemDto
        CreateMap<PromoProgramaPromotor, InscripcionListItemDto>()
            .ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaInscripcion))
            .ForMember(dest => dest.PromotorId, opt => opt.Ignore())
            .ForMember(dest => dest.PromotorNombre, opt => opt.Ignore())
            .ForMember(dest => dest.TipoPromotorNombre, opt => opt.Ignore())
            .ForMember(dest => dest.PromotorEmailContacto, opt => opt.Ignore())
            .ForMember(dest => dest.PromotorUrlInstagram, opt => opt.Ignore())
            .ForMember(dest => dest.PromotorUrlTikTok, opt => opt.Ignore())
            .ForMember(dest => dest.PromotorUrlSitioWeb, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.Ignore())
            .ForMember(dest => dest.CodigoReferido, opt => opt.Ignore());

        // PromoProgramaPromotor -> MiInscripcionDto
        CreateMap<PromoProgramaPromotor, MiInscripcionDto>()
            .ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaInscripcion))
            .ForMember(dest => dest.ProgramaId, opt => opt.MapFrom(src => src.ProgramaId.Value))
            .ForMember(dest => dest.UrlTrackingPersonalizada, opt => opt.Ignore())
            .ForMember(dest => dest.CodigoReferido, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.Ignore())
            .ForMember(dest => dest.Tareas, opt => opt.Ignore())
            .ForMember(dest => dest.ProgramaTitulo, opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.TipoPromoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.ImporteComisionPorcentaje, opt => opt.Ignore())
            .ForMember(dest => dest.ImporteComisionFija, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore());

        // PromoPrograma -> ProgramaExplorarItemDto
        CreateMap<PromoPrograma, ProgramaExplorarItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.ArtistaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.TipoPromoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroTareas, opt => opt.Ignore())
            .ForMember(dest => dest.CampaniaTitulo, opt => opt.Ignore())
            .ForMember(dest => dest.MiEstado, opt => opt.Ignore());

        // PromoTarea -> TareaResumenDto
        CreateMap<PromoTarea, TareaResumenDto>()
            .ForMember(dest => dest.TipoEventoPromoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore());
    }
}
