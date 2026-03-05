using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class PropuestaCrowdsourcingProfile : Profile
{
    public PropuestaCrowdsourcingProfile()
    {
        CreateMap<PropuestaCrowdsourcing, PropuestaCrowdsourcingDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.EstadoPropuestaNombre, opt => opt.Ignore());

        // Command -> Entity (para crear propuesta)
        CreateMap<CreatePropuestaCommand, PropuestaCrowdsourcing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.NecesidadId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PerfilProfesionalId, opt => opt.Ignore())
            .ForMember(dest => dest.EstadoPropuestaId, opt => opt.Ignore())
            .ForMember(dest => dest.MotivoRechazo, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
            .ForMember(dest => dest.Necesidad, opt => opt.Ignore())
            .ForMember(dest => dest.Acuerdos, opt => opt.Ignore());

        // Entity -> PropuestaCreatedResultDto
        CreateMap<PropuestaCrowdsourcing, PropuestaCreatedResultDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.NecesidadTitulo, opt => opt.Ignore())
            .ForMember(dest => dest.EstadoPropuestaNombre, opt => opt.Ignore());

        // Entity -> MiPropuestaListDto
        CreateMap<PropuestaCrowdsourcing, MiPropuestaListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.NecesidadTitulo, opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.EstadoPropuestaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.AcuerdoId, opt => opt.Ignore());

        // Entity -> RetirarPropuestaResultDto
        CreateMap<PropuestaCrowdsourcing, RetirarPropuestaResultDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.EstadoPropuestaNombre, opt => opt.Ignore());
    }
}
