using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class EntregableProfile : Profile
{
    public EntregableProfile()
    {
        CreateMap<AcuerdoCrowdsourcingEntregable, EntregableDto>()
            .ForMember(dest => dest.EstadoEntregableNombre, opt => opt.Ignore());
    }
}
