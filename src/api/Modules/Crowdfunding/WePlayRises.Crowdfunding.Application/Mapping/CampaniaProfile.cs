using AutoMapper;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Mapping;

public class CampaniaProfile : Profile
{
    public CampaniaProfile()
    {
        // ---------------------------------------------------------------------
        // Entity -> DTO (full)
        // ---------------------------------------------------------------------
        CreateMap<CampaniaCrowdfunding, CampaniaDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.ArtistaId,
                       opt => opt.MapFrom(src => src.ArtistaId.Value))
            .ForMember(dest => dest.ProyectoArtisticoId,
                       opt => opt.MapFrom(src => src.ProyectoArtisticoId.HasValue ? src.ProyectoArtisticoId.Value.Value : (Guid?)null));

        // ---------------------------------------------------------------------
        // Entity -> DTO (list simplified)
        // ---------------------------------------------------------------------
        CreateMap<CampaniaCrowdfunding, CampaniaListDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.ArtistaId,
                       opt => opt.MapFrom(src => src.ArtistaId.Value))
            .ForMember(dest => dest.ProyectoArtisticoId,
                       opt => opt.MapFrom(src => src.ProyectoArtisticoId.HasValue ? src.ProyectoArtisticoId.Value.Value : (Guid?)null))
            .ForMember(dest => dest.TieneCrowdsourcing, opt => opt.Ignore())
            .ForMember(dest => dest.TieneCrowdpromotion, opt => opt.Ignore());

        // ---------------------------------------------------------------------
        // Command -> Entity
        // ---------------------------------------------------------------------
        // ---------------------------------------------------------------------
        // Entity -> CampaniaDetailDto (public detail view)
        // ---------------------------------------------------------------------
        CreateMap<CampaniaCrowdfunding, CampaniaDetailDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.ArtistaId,
                       opt => opt.MapFrom(src => src.ArtistaId.Value))
            .ForMember(dest => dest.ProyectoArtisticoId,
                       opt => opt.MapFrom(src => src.ProyectoArtisticoId.HasValue ? src.ProyectoArtisticoId.Value.Value : (Guid?)null))
            .ForMember(dest => dest.PorcentajeProgreso, opt => opt.Ignore())
            .ForMember(dest => dest.DiasRestantes, opt => opt.Ignore())
            .ForMember(dest => dest.MonedaSimbolo, opt => opt.Ignore())
            .ForMember(dest => dest.EstadoCampaniaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaNombre, opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaImagenUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Rewards, opt => opt.Ignore())
            .ForMember(dest => dest.BackingsRecientes, opt => opt.Ignore())
            .ForMember(dest => dest.TotalBackers, opt => opt.Ignore())
            .ForMember(dest => dest.TieneCrowdsourcing, opt => opt.Ignore())
            .ForMember(dest => dest.TieneCrowdpromotion, opt => opt.Ignore());

        // ---------------------------------------------------------------------
        // Command -> Entity
        // ---------------------------------------------------------------------
        CreateMap<CreateCampaniaCommand, CampaniaCrowdfunding>()
            .ForMember(dest => dest.ArtistaId,
                       opt => opt.MapFrom(src => new BuildingBlocks.EntityFramework.StronglyTypedIds.ArtistaId(src.ArtistaId)))
            .ForMember(dest => dest.ProyectoArtisticoId,
                       opt => opt.MapFrom(src => src.ProyectoArtisticoId.HasValue
                           ? new BuildingBlocks.EntityFramework.StronglyTypedIds.ProyectoArtisticoId(src.ProyectoArtisticoId.Value)
                           : (BuildingBlocks.EntityFramework.StronglyTypedIds.ProyectoArtisticoId?)null))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EstadoCampaniaId, opt => opt.Ignore())
            .ForMember(dest => dest.ImportePledgedActual, opt => opt.Ignore())
            .ForMember(dest => dest.PorcentajeComisionPlataforma, opt => opt.Ignore())
            .ForMember(dest => dest.FechaPublicacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCierre, opt => opt.Ignore())
            .ForMember(dest => dest.Borrado, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
            .ForMember(dest => dest.Rewards, opt => opt.Ignore())
            .ForMember(dest => dest.Pedidos, opt => opt.Ignore())
            .ForMember(dest => dest.Payouts, opt => opt.Ignore())
            .ForMember(dest => dest.Updates, opt => opt.Ignore())
            .ForMember(dest => dest.Comentarios, opt => opt.Ignore())
            .ForMember(dest => dest.StretchGoals, opt => opt.Ignore());
    }
}
