using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class MilestoneProfile : Profile
{
    public MilestoneProfile()
    {
        CreateMap<AcuerdoCrowdsourcingMilestone, MilestoneDto>()
            .ForMember(dest => dest.PorcentajeParcial, opt => opt.Ignore())
            .ForMember(dest => dest.Entregables, opt => opt.MapFrom(src => src.Entregables));
    }
}
