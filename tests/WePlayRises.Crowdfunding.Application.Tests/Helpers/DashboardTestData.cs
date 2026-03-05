using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Helpers;

public static class DashboardTestData
{
    // Handler does Guid.Parse(request.UserId) to build ArtistaId,
    // so TestUserId MUST be a valid GUID and match TestArtistaGuid.
    public static readonly Guid TestArtistaGuid = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly string TestUserId = TestArtistaGuid.ToString();
    public static readonly Guid TestCampaniaGuid = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static Artista CreateArtista(
        Guid? artistaId = null,
        string userId = "test-user-id-123",
        string nombreArtistico = "Test Artist")
    {
        return new Artista
        {
            Id = new ArtistaId(artistaId ?? TestArtistaGuid),
            UserIdPropietario = userId,
            NombreArtistico = nombreArtistico,
            Descripcion = "Test artist description",
            FechaCreacion = DateTime.UtcNow.AddDays(-30)
        };
    }

    public static CampaniaCrowdfunding CreateCampania(
        Guid? campaniaId = null,
        Guid? artistaId = null,
        int estadoId = 2,
        decimal importeObjetivo = 5000m,
        decimal importePledged = 1000m)
    {
        return new CampaniaCrowdfunding
        {
            Id = new CampaniaCrowdfundingId(campaniaId ?? TestCampaniaGuid),
            ArtistaId = new ArtistaId(artistaId ?? TestArtistaGuid),
            Titulo = "Test Campania",
            Subtitulo = "Test subtitle",
            DescripcionCorta = "Test description",
            MonedaId = 1,
            ImporteObjetivo = importeObjetivo,
            ImportePledgedActual = importePledged,
            TipoFinanciacionId = 1,
            EstadoCampaniaId = estadoId,
            FechaInicio = DateTime.UtcNow.AddDays(-10),
            FechaFin = DateTime.UtcNow.AddDays(20),
            FechaCreacion = DateTime.UtcNow.AddDays(-10),
            Borrado = false,
            Rewards = new List<CampaniaCrowdfundingReward>(),
            Pedidos = new List<PedidoCrowdfunding>()
        };
    }

    public static PedidoCrowdfunding CreatePedido(
        Guid? pedidoId = null,
        Guid? campaniaId = null,
        decimal monto = 25m,
        bool permitirMostrarNombre = true)
    {
        return new PedidoCrowdfunding
        {
            Id = new PedidoCrowdfundingId(pedidoId ?? Guid.NewGuid()),
            CampaniaId = new CampaniaCrowdfundingId(campaniaId ?? TestCampaniaGuid),
            UserId = "backer-user-id",
            EstadoPedidoId = 3,
            MonedaId = 1,
            ImporteSubtotal = monto,
            ImporteTotal = monto,
            PermitirMostrarNombre = permitirMostrarNombre,
            ComentarioBacker = "Test backing",
            FechaCreacion = DateTime.UtcNow,
            Lineas = new List<PedidoCrowdfundingLinea>(),
            Aportaciones = new List<AportacionCrowdfunding>()
        };
    }
}
