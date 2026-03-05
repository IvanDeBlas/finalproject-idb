using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class ValoracionProfile : Profile
{
    public ValoracionProfile()
    {
        CreateMap<CreateValoracionCommand, ValoracionCrowdsourcing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AcuerdoId, opt => opt.Ignore())
            .ForMember(dest => dest.UserIdAutor, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserIdValorado, opt => opt.Ignore())
            .ForMember(dest => dest.Puntuacion, opt => opt.MapFrom(src => (byte)src.Puntuacion))
            .ForMember(dest => dest.TipoValoracionId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Acuerdo, opt => opt.Ignore());

        CreateMap<ValoracionCrowdsourcing, ValoracionCreatedResultDto>()
            .ForMember(dest => dest.Puntuacion, opt => opt.MapFrom(src => (int)src.Puntuacion));

        CreateMap<ValoracionCrowdsourcing, ValoracionListItemDto>()
            .ForMember(dest => dest.Puntuacion, opt => opt.MapFrom(src => (int)src.Puntuacion))
            .ForMember(dest => dest.AutorNombre, opt => opt.Ignore())
            .ForMember(dest => dest.AutorImagenUrl, opt => opt.Ignore())
            .ForMember(dest => dest.AcuerdoTituloInterno, opt => opt.Ignore());
    }
}
