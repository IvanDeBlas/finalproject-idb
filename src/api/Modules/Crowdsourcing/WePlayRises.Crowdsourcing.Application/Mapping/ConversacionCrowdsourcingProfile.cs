using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class ConversacionCrowdsourcingProfile : Profile
{
    public ConversacionCrowdsourcingProfile()
    {
        CreateMap<CreateConversacionCommand, ConversacionCrowdsourcing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserIdCreador, opt => opt.Ignore())
            .ForMember(dest => dest.UserIdDestinatario, opt => opt.Ignore())
            .ForMember(dest => dest.NecesidadId, opt => opt.Ignore())
            .ForMember(dest => dest.AcuerdoId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaUltimoMensaje, opt => opt.Ignore())
            .ForMember(dest => dest.Necesidad, opt => opt.Ignore())
            .ForMember(dest => dest.Acuerdo, opt => opt.Ignore())
            .ForMember(dest => dest.Mensajes, opt => opt.Ignore());
    }
}
