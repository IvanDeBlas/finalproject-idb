using AutoMapper;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Commands;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Mapping;

public class PedidoProfile : Profile
{
    public PedidoProfile()
    {
        // ---------------------------------------------------------------------
        // Entity -> DTO (full)
        // ---------------------------------------------------------------------
        CreateMap<PedidoCrowdfunding, PedidoDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.CampaniaId,
                       opt => opt.MapFrom(src => src.CampaniaId.Value))
            .ForMember(dest => dest.FanProfileId,
                       opt => opt.MapFrom(src => src.FanProfileId.HasValue ? src.FanProfileId.Value.Value : (Guid?)null));

        // ---------------------------------------------------------------------
        // Entity -> DTO (list simplified)
        // ---------------------------------------------------------------------
        CreateMap<PedidoCrowdfunding, PedidoListDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.CampaniaId,
                       opt => opt.MapFrom(src => src.CampaniaId.Value));

        // ---------------------------------------------------------------------
        // PedidoLinea -> DTO
        // ---------------------------------------------------------------------
        CreateMap<PedidoCrowdfundingLinea, PedidoLineaDto>()
            .ForMember(dest => dest.RewardId,
                       opt => opt.MapFrom(src => src.RewardId.Value));

        // ---------------------------------------------------------------------
        // Command -> Entity
        // ---------------------------------------------------------------------
        CreateMap<CreatePedidoCommand, PedidoCrowdfunding>()
            .ForMember(dest => dest.CampaniaId,
                       opt => opt.MapFrom(src => new CampaniaCrowdfundingId(src.CampaniaId)))
            .ForMember(dest => dest.FanProfileId,
                       opt => opt.MapFrom(src => src.FanProfileId.HasValue
                           ? new FanProfileId(src.FanProfileId.Value)
                           : (FanProfileId?)null))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EstadoPedidoId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
            .ForMember(dest => dest.Campania, opt => opt.Ignore())
            .ForMember(dest => dest.Lineas, opt => opt.Ignore())
            .ForMember(dest => dest.Aportaciones, opt => opt.Ignore());
    }
}
