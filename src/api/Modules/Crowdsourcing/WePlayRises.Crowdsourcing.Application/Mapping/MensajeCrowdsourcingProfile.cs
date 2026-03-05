using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class MensajeCrowdsourcingProfile : Profile
{
    public MensajeCrowdsourcingProfile()
    {
        CreateMap<MensajeCrowdsourcing, MensajeDto>()
            .ForMember(dest => dest.RemitenteNombre, opt => opt.Ignore())
            .ForMember(dest => dest.EsPropio, opt => opt.Ignore());
    }
}
