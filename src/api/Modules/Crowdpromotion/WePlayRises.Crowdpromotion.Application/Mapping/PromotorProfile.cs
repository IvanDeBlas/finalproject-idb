using AutoMapper;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Mapping;

public class PromotorProfile : Profile
{
    public PromotorProfile()
    {
        CreateMap<CreatePromotorCommand, Domain.Model.Promotor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.FanProfileId, opt => opt.Ignore())
            .ForMember(dest => dest.EsActivo, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
            .ForMember(dest => dest.SeguidoresTotales, opt => opt.Ignore())
            .ForMember(dest => dest.Programas, opt => opt.Ignore())
            .ForMember(dest => dest.Wallets, opt => opt.Ignore());

        CreateMap<Domain.Model.Promotor, PromotorCreatedResultDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.TipoPromotorNombre, opt => opt.Ignore());

        CreateMap<Domain.Model.Promotor, PromotorDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.TipoPromotorNombre, opt => opt.Ignore())
            .ForMember(dest => dest.TotalProgramasActivos, opt => opt.Ignore())
            .ForMember(dest => dest.TotalComisionesGanadas, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaComisiones, opt => opt.Ignore());

        CreateMap<Domain.Model.Promotor, PromotorUpdatedResultDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.FechaActualizacion,
                opt => opt.MapFrom(src => src.FechaActualizacion ?? DateTime.UtcNow));

        CreateMap<Domain.Model.Promotor, PromotorDesactivadoResultDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.ProgramasDadosDeBaja, opt => opt.Ignore());
    }
}
