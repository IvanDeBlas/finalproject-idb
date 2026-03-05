using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Helpers;

public static class CampaniaTestData
{
    public static CampaniaCrowdfunding CreateValidBorrador(
        Guid? artistaId = null,
        Guid? campaniaId = null,
        string titulo = "Test Campania",
        decimal importeObjetivo = 5000m)
    {
        var aId = artistaId ?? Guid.NewGuid();
        var cId = campaniaId ?? Guid.NewGuid();

        return new CampaniaCrowdfunding
        {
            Id = new CampaniaCrowdfundingId(cId),
            ArtistaId = new ArtistaId(aId),
            Titulo = titulo,
            Subtitulo = "Test subtitle",
            DescripcionCorta = "Test description",
            MonedaId = 1,
            ImporteObjetivo = importeObjetivo,
            ImportePledgedActual = 0,
            TipoFinanciacionId = 1,
            EstadoCampaniaId = 1, // Borrador
            FechaFin = DateTime.UtcNow.AddDays(30),
            FechaCreacion = DateTime.UtcNow,
            Borrado = false,
        };
    }

    public static CampaniaCrowdfunding CreatePublicada(
        Guid? artistaId = null,
        Guid? campaniaId = null)
    {
        var entity = CreateValidBorrador(artistaId, campaniaId);
        entity.EstadoCampaniaId = 2; // Publicada
        entity.FechaPublicacion = DateTime.UtcNow;
        return entity;
    }
}
