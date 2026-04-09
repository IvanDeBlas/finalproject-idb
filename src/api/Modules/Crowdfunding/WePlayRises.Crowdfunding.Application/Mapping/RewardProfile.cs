using AutoMapper;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Mapping;

public class RewardProfile : Profile
{
    public RewardProfile()
    {
        // ---------------------------------------------------------------------
        // Entity -> DTO (full)
        // ---------------------------------------------------------------------
        CreateMap<CampaniaCrowdfundingReward, RewardDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.CampaniaId,
                       opt => opt.MapFrom(src => src.CampaniaId.Value));

        // ---------------------------------------------------------------------
        // Entity -> DTO (list simplified)
        // ---------------------------------------------------------------------
        CreateMap<CampaniaCrowdfundingReward, RewardListDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.CampaniaId,
                       opt => opt.MapFrom(src => src.CampaniaId.Value));

        // ---------------------------------------------------------------------
        // Entity -> RewardPublicDto (public view with stock info)
        // ---------------------------------------------------------------------
        CreateMap<CampaniaCrowdfundingReward, RewardPublicDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.CantidadVendida, opt => opt.Ignore())
            .ForMember(dest => dest.Disponible, opt => opt.Ignore());

        // ---------------------------------------------------------------------
        // Command -> Entity
        // ---------------------------------------------------------------------
        CreateMap<CreateRewardCommand, CampaniaCrowdfundingReward>()
            .ForMember(dest => dest.CampaniaId,
                       opt => opt.MapFrom(src => new BuildingBlocks.EntityFramework.StronglyTypedIds.CampaniaCrowdfundingId(src.CampaniaId)))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EsActivo, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
            .ForMember(dest => dest.Campania, opt => opt.Ignore())
            .ForMember(dest => dest.Lineas, opt => opt.Ignore());
    }
}
