using AutoMapper;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Mapping;

public class PromoTareaProfile : Profile
{
    public PromoTareaProfile()
    {
        CreateMap<CreatePromoProgramaCommand.CreatePromoTareaItem, PromoTarea>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProgramaId, opt => opt.Ignore())
            .ForMember(dest => dest.Orden, opt => opt.Ignore())
            .ForMember(dest => dest.EsActivo, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Programa, opt => opt.Ignore())
            .ForMember(dest => dest.TareasPromotor, opt => opt.Ignore())
            .ForMember(dest => dest.InstruccionesUrl,
                opt => opt.MapFrom(src => src.UrlInstrucciones))
            .ForMember(dest => dest.FechaInicio,
                opt => opt.MapFrom(src => src.FechaInicio.HasValue
                    ? src.FechaInicio.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))
            .ForMember(dest => dest.FechaFin,
                opt => opt.MapFrom(src => src.FechaFin.HasValue
                    ? src.FechaFin.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null));

        CreateMap<UpdatePromoProgramaCommand.UpdatePromoTareaItem, PromoTarea>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProgramaId, opt => opt.Ignore())
            .ForMember(dest => dest.Orden, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Programa, opt => opt.Ignore())
            .ForMember(dest => dest.TareasPromotor, opt => opt.Ignore())
            .ForMember(dest => dest.InstruccionesUrl,
                opt => opt.MapFrom(src => src.UrlInstrucciones))
            .ForMember(dest => dest.EsActivo,
                opt => opt.MapFrom(src => src.EsActivo))
            .ForMember(dest => dest.FechaInicio,
                opt => opt.MapFrom(src => src.FechaInicio.HasValue
                    ? src.FechaInicio.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))
            .ForMember(dest => dest.FechaFin,
                opt => opt.MapFrom(src => src.FechaFin.HasValue
                    ? src.FechaFin.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null));

        CreateMap<PromoTarea, PromoTareaDetailDto>()
            .ForMember(dest => dest.TipoRewardId, opt => opt.MapFrom(src => src.TipoRewardId ?? 0))
            .ForMember(dest => dest.UrlInstrucciones,
                opt => opt.MapFrom(src => src.InstruccionesUrl))
            .ForMember(dest => dest.TipoEventoPromoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.TipoRewardNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.CompletadosPorPromotores, opt => opt.Ignore())
            .ForMember(dest => dest.FechaInicio,
                opt => opt.MapFrom(src => src.FechaInicio.HasValue
                    ? DateOnly.FromDateTime(src.FechaInicio.Value)
                    : (DateOnly?)null))
            .ForMember(dest => dest.FechaFin,
                opt => opt.MapFrom(src => src.FechaFin.HasValue
                    ? DateOnly.FromDateTime(src.FechaFin.Value)
                    : (DateOnly?)null));

        // PromoTarea -> MisTareasItemDto
        CreateMap<PromoTarea, MisTareasItemDto>()
            .ForMember(dest => dest.TareaId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Titulo))
            .ForMember(dest => dest.InstruccionesUrl, opt => opt.MapFrom(src => src.InstruccionesUrl))
            .ForMember(dest => dest.TipoEventoPromoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.TipoRewardNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MiEstado, opt => opt.Ignore());

        // PromoTareaPromotor -> CompletarTareaResponseDto
        CreateMap<PromoTareaPromotor, CompletarTareaResponseDto>()
            .ForMember(dest => dest.TareaPromotorId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EstadoTareaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.VecesCompletada, opt => opt.Ignore())
            .ForMember(dest => dest.FechaUltimaCompletada, opt => opt.MapFrom(src => src.FechaCompletado));

        // PromoTareaPromotor -> RechazarTareaResponseDto
        CreateMap<PromoTareaPromotor, RechazarTareaResponseDto>()
            .ForMember(dest => dest.TareaPromotorId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EstadoTareaNombre, opt => opt.Ignore());
    }
}
