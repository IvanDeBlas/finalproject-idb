using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class PlantillaProyectoProfile : Profile
{
    public PlantillaProyectoProfile()
    {
        // Entity -> DTO List (para galeria)
        CreateMap<PlantillaProyecto, PlantillaProyectoListDto>()
            .ForMember(dest => dest.PrecioMinTotal,
                opt => opt.MapFrom(src => src.Necesidades.Sum(n => n.PrecioMinOrientativo ?? 0)))
            .ForMember(dest => dest.PrecioMaxTotal,
                opt => opt.MapFrom(src => src.Necesidades.Sum(n => n.PrecioMaxOrientativo ?? 0)))
            .ForMember(dest => dest.Moneda,
                opt => opt.MapFrom(src => src.Necesidades.FirstOrDefault() != null
                    ? src.Necesidades.First().MonedaId
                    : 1))
            .ForMember(dest => dest.CantidadNecesidades,
                opt => opt.MapFrom(src => src.Necesidades.Count))
            .ForMember(dest => dest.Fases,
                opt => opt.MapFrom(src => src.Necesidades
                    .Select(n => n.Fase)
                    .Distinct()
                    .OrderBy(f => f)
                    .ToList()));

        // Entity -> DTO Detail (con necesidades)
        CreateMap<PlantillaProyecto, PlantillaProyectoDto>()
            .ForMember(dest => dest.Necesidades,
                opt => opt.MapFrom(src => src.Necesidades.OrderBy(n => n.Orden)))
            .ForMember(dest => dest.Resumen,
                opt => opt.MapFrom(src => new PlantillaResumenDto
                {
                    PrecioMinTotal = src.Necesidades.Sum(n => n.PrecioMinOrientativo ?? 0),
                    PrecioMaxTotal = src.Necesidades.Sum(n => n.PrecioMaxOrientativo ?? 0),
                    Moneda = src.Necesidades.FirstOrDefault() != null
                        ? src.Necesidades.First().MonedaId
                        : 1,
                    CantidadNecesidadesAlta = src.Necesidades.Count(n => n.Prioridad == "Alta"),
                    CantidadNecesidadesMedia = src.Necesidades.Count(n => n.Prioridad == "Media"),
                    CantidadNecesidadesBaja = src.Necesidades.Count(n => n.Prioridad == "Baja")
                }));

        // PlantillaProyectoNecesidad -> DTO
        CreateMap<PlantillaProyectoNecesidad, PlantillaProyectoNecesidadDto>()
            .ForMember(dest => dest.Moneda,
                opt => opt.MapFrom(src => src.MonedaId))
            .ForMember(dest => dest.RolProfesional,
                opt => opt.MapFrom(src => src.RolProfesional));
    }
}
