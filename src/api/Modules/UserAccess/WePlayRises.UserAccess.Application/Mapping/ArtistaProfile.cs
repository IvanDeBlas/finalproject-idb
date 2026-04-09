using AutoMapper;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Features.Artistas.Commands;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Mapping;

public class ArtistaProfile : Profile
{
    public ArtistaProfile()
    {
        // Command -> Entity
        CreateMap<CreateArtistaCommand, Artista>()
            .ForMember(dest => dest.UserIdPropietario,
                       opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.ImagenPerfilUrl,
                       opt => opt.MapFrom(src => src.ImagenUrl))
            .ForMember(dest => dest.Id,
                       opt => opt.Ignore()) // Generated in Handler
            .ForMember(dest => dest.FechaCreacion,
                       opt => opt.Ignore()) // Set in Service
            .ForMember(dest => dest.FechaActualizacion,
                       opt => opt.Ignore())
            .ForMember(dest => dest.UrlSitioWeb,
                       opt => opt.Ignore())
            .ForMember(dest => dest.UrlInstagram,
                       opt => opt.Ignore())
            .ForMember(dest => dest.UrlYouTube,
                       opt => opt.Ignore())
            .ForMember(dest => dest.UrlSpotify,
                       opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaMiembros,
                       opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaFans,
                       opt => opt.Ignore())
            .ForMember(dest => dest.ProyectosArtisticos,
                       opt => opt.Ignore());

        // Entity -> DTO (full)
        CreateMap<Artista, ArtistaDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value)) // ArtistaId -> Guid
            .ForMember(dest => dest.UserId,
                       opt => opt.MapFrom(src => src.UserIdPropietario))
            .ForMember(dest => dest.ImagenUrl,
                       opt => opt.MapFrom(src => src.ImagenPerfilUrl));

        // Entity -> DTO (list simplified)
        CreateMap<Artista, ArtistaListDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.ImagenUrl,
                       opt => opt.MapFrom(src => src.ImagenPerfilUrl));
    }
}
