using AutoMapper;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Mapping;

public class PromotorWalletProfile : Profile
{
    public PromotorWalletProfile()
    {
        // PromotorWallet -> PromotorWalletDto
        // MinimoRetiro: assigned in Handler from IConfiguration
        // MonedaNombre: assigned in Handler (Maestra is in CoreContext)
        CreateMap<PromotorWallet, PromotorWalletDto>()
            .ForMember(dest => dest.WalletId,
                       opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MonedaNombre,
                       opt => opt.Ignore())
            .ForMember(dest => dest.MinimoRetiro,
                       opt => opt.Ignore());

        // PromotorWalletTransaccion -> WalletTransaccionItemDto
        // EstadoTransaccionNombre, TipoRewardNombre: assigned in Handler (Maestra is in CoreContext)
        CreateMap<PromotorWalletTransaccion, WalletTransaccionItemDto>()
            .ForMember(dest => dest.EstadoTransaccionNombre,
                       opt => opt.Ignore())
            .ForMember(dest => dest.TipoRewardNombre,
                       opt => opt.Ignore());
    }
}
