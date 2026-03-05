using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class AcuerdoProfile : Profile
{
    public AcuerdoProfile()
    {
        CreateMap<AcuerdoCrowdsourcing, AcuerdoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.EstadoAcuerdoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.Artista, opt => opt.Ignore())
            .ForMember(dest => dest.Profesional, opt => opt.Ignore())
            .ForMember(dest => dest.Necesidad, opt => opt.Ignore())
            .ForMember(dest => dest.ConversacionId, opt => opt.Ignore())
            .ForMember(dest => dest.ImporteAsignado, opt => opt.Ignore())
            .ForMember(dest => dest.PorcentajeAsignado, opt => opt.Ignore())
            .ForMember(dest => dest.MiRol, opt => opt.Ignore())
            .ForMember(dest => dest.Timeline, opt => opt.Ignore())
            .ForMember(dest => dest.Milestones, opt => opt.MapFrom(src => src.Milestones.OrderBy(m => m.Orden)));
    }
}
