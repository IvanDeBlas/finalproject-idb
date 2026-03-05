using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class NecesidadCrowdsourcingProfile : Profile
{
    public NecesidadCrowdsourcingProfile()
    {
        // Command -> Entity (para crear)
        CreateMap<CreateNecesidadCommand, NecesidadCrowdsourcing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaId, opt => opt.Ignore())
            .ForMember(dest => dest.ProyectoArtisticoId, opt => opt.Ignore())
            .ForMember(dest => dest.EstadoNecesidadId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
            .ForMember(dest => dest.MotivoCierre, opt => opt.Ignore())
            .ForMember(dest => dest.Propuestas, opt => opt.Ignore())
            .ForMember(dest => dest.Acuerdos, opt => opt.Ignore())
            .ForMember(dest => dest.Conversaciones, opt => opt.Ignore());

        // Entity -> ListDto
        CreateMap<NecesidadCrowdsourcing, NecesidadCrowdsourcingListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.EstadoNecesidadNombre, opt => opt.Ignore())
            .ForMember(dest => dest.TipoNecesidadNombre, opt => opt.Ignore())
            .ForMember(dest => dest.ModalidadTrabajoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroPropuestas, opt => opt.MapFrom(src => src.Propuestas.Count));

        // Entity -> DTO completo
        CreateMap<NecesidadCrowdsourcing, NecesidadCrowdsourcingDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.ProyectoArtisticoId, opt => opt.MapFrom(src => src.ProyectoArtisticoId.Value))
            .ForMember(dest => dest.EstadoNecesidadNombre, opt => opt.Ignore())
            .ForMember(dest => dest.TipoNecesidadNombre, opt => opt.Ignore())
            .ForMember(dest => dest.ModalidadTrabajoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.ProyectoArtisticoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroPropuestas, opt => opt.MapFrom(src => src.Propuestas.Count))
            .ForMember(dest => dest.Propuestas, opt => opt.MapFrom(src => src.Propuestas));

        // Entity -> NecesidadPublicaListDto (vista profesional - listado)
        CreateMap<NecesidadCrowdsourcing, NecesidadPublicaListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.TipoNecesidadNombre, opt => opt.Ignore())
            .ForMember(dest => dest.ModalidadTrabajoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.EsUrgente, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroPropuestas, opt => opt.Ignore());

        // Entity -> NecesidadPublicaDto (vista profesional - detalle)
        CreateMap<NecesidadCrowdsourcing, NecesidadPublicaDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.TipoNecesidadNombre, opt => opt.Ignore())
            .ForMember(dest => dest.EstadoNecesidadNombre, opt => opt.Ignore())
            .ForMember(dest => dest.ModalidadTrabajoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroPropuestas, opt => opt.Ignore())
            .ForMember(dest => dest.Artista, opt => opt.Ignore())
            .ForMember(dest => dest.YaPropuso, opt => opt.Ignore())
            .ForMember(dest => dest.EsPropietario, opt => opt.Ignore())
            .ForMember(dest => dest.TienePerfilProfesional, opt => opt.Ignore());
    }
}
