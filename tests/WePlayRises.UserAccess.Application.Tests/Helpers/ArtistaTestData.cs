using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Tests.Helpers;

public static class ArtistaTestData
{
    public static Artista CreateValid(
        Guid? artistaId = null,
        string userId = "user-123",
        string nombreArtistico = "Test Artist")
    {
        var aId = artistaId ?? Guid.NewGuid();

        return new Artista
        {
            Id = new ArtistaId(aId),
            UserIdPropietario = userId,
            NombreArtistico = nombreArtistico,
            Descripcion = "Test description",
            Pais = "Spain",
            Ciudad = "Madrid",
            ImagenPerfilUrl = "https://example.com/image.jpg",
            FechaCreacion = DateTime.UtcNow,
        };
    }

    public static ArtistaDto CreateValidDto(
        Guid? artistaId = null,
        string userId = "user-123",
        string nombreArtistico = "Test Artist")
    {
        var aId = artistaId ?? Guid.NewGuid();

        return new ArtistaDto
        {
            Id = aId,
            UserId = userId,
            NombreArtistico = nombreArtistico,
            Descripcion = "Test description",
            Pais = "Spain",
            Ciudad = "Madrid",
            ImagenUrl = "https://example.com/image.jpg",
            FechaCreacion = DateTime.UtcNow,
        };
    }
}
