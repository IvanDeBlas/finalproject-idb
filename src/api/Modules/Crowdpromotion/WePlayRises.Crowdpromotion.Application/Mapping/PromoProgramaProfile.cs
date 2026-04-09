using AutoMapper;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Mapping;

public class PromoProgramaProfile : Profile
{
    public PromoProgramaProfile()
    {
        CreateMap<CreatePromoProgramaCommand, PromoPrograma>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaId, opt => opt.Ignore())
            .ForMember(dest => dest.EsActivo, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
            .ForMember(dest => dest.Tareas, opt => opt.Ignore())
            .ForMember(dest => dest.Promotores, opt => opt.Ignore())
            .ForMember(dest => dest.Eventos, opt => opt.Ignore())
            .ForMember(dest => dest.PresupuestoTotal, opt => opt.Ignore())
            .ForMember(dest => dest.ComisionPorConversion, opt => opt.Ignore())
            .ForMember(dest => dest.ComisionPorClick, opt => opt.Ignore())
            .ForMember(dest => dest.CampaniaCrowdfundingId, opt => opt.Ignore())
            .ForMember(dest => dest.ProyectoArtisticoId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaInicio,
                opt => opt.MapFrom(src => src.FechaInicio.HasValue
                    ? src.FechaInicio.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))
            .ForMember(dest => dest.FechaFin,
                opt => opt.MapFrom(src => src.FechaFin.HasValue
                    ? src.FechaFin.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null));

        CreateMap<PromoPrograma, PromoProgramaCreatedResultDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.TipoPromoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.TareasCreadas, opt => opt.Ignore());

        CreateMap<PromoPrograma, PromoProgramaListItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.TipoPromoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.CampaniaTitulo, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroPromotores, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroTareas, opt => opt.Ignore())
            .ForMember(dest => dest.FechaInicio,
                opt => opt.MapFrom(src => src.FechaInicio.HasValue
                    ? DateOnly.FromDateTime(src.FechaInicio.Value)
                    : (DateOnly?)null))
            .ForMember(dest => dest.FechaFin,
                opt => opt.MapFrom(src => src.FechaFin.HasValue
                    ? DateOnly.FromDateTime(src.FechaFin.Value)
                    : (DateOnly?)null));

        CreateMap<PromoPrograma, PromoProgramaDetailDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.CampaniaCrowdfundingId,
                opt => opt.MapFrom(src => src.CampaniaCrowdfundingId.HasValue ? src.CampaniaCrowdfundingId.Value.Value : (Guid?)null))
            .ForMember(dest => dest.ProyectoArtisticoId,
                opt => opt.MapFrom(src => src.ProyectoArtisticoId.HasValue ? src.ProyectoArtisticoId.Value.Value : (Guid?)null))
            .ForMember(dest => dest.MonedaId, opt => opt.MapFrom(src => src.MonedaId ?? 0))
            .ForMember(dest => dest.TipoPromoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.CampaniaTitulo, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.Tareas, opt => opt.Ignore())
            .ForMember(dest => dest.Promotores, opt => opt.Ignore())
            .ForMember(dest => dest.Resumen, opt => opt.Ignore())
            .ForMember(dest => dest.FechaInicio,
                opt => opt.MapFrom(src => src.FechaInicio.HasValue
                    ? DateOnly.FromDateTime(src.FechaInicio.Value)
                    : (DateOnly?)null))
            .ForMember(dest => dest.FechaFin,
                opt => opt.MapFrom(src => src.FechaFin.HasValue
                    ? DateOnly.FromDateTime(src.FechaFin.Value)
                    : (DateOnly?)null));

        CreateMap<PromoPrograma, PromoProgramaUpdatedResultDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.FechaActualizacion,
                opt => opt.MapFrom(src => src.FechaActualizacion ?? DateTime.UtcNow));

        CreateMap<PromoPrograma, PromoProgramaDesactivadoResultDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.TareasDesactivadas, opt => opt.Ignore());
    }
}
