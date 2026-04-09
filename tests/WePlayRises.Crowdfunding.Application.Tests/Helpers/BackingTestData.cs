using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Features.Backings.Commands;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Helpers;

public static class BackingTestData
{
    public static CreateBackingCommand CreateValidCommand(
        Guid? campaniaId = null,
        Guid? rewardId = null,
        decimal monto = 25m,
        string? userId = "user-123",
        string? userName = "Test User")
    {
        return new CreateBackingCommand
        {
            CampaniaId = campaniaId ?? Guid.NewGuid(),
            RewardId = rewardId,
            Monto = monto,
            Mensaje = "Mucha suerte!",
            EsAnonimo = false,
            UserId = userId,
            UserName = userName
        };
    }

    public static CampaniaCrowdfunding CreatePublicadaCampania(
        Guid? campaniaId = null,
        Guid? artistaId = null,
        bool permiteAnonimas = true)
    {
        var cId = campaniaId ?? Guid.NewGuid();
        var aId = artistaId ?? Guid.NewGuid();

        return new CampaniaCrowdfunding
        {
            Id = new CampaniaCrowdfundingId(cId),
            ArtistaId = new ArtistaId(aId),
            Titulo = "Test Campania Publicada",
            Subtitulo = "Test subtitle",
            DescripcionCorta = "Test description",
            MonedaId = 1,
            ImporteObjetivo = 5000m,
            ImportePledgedActual = 1000m,
            TipoFinanciacionId = 1,
            EstadoCampaniaId = 2, // Publicada
            PermiteAportacionesAnonimas = permiteAnonimas,
            FechaFin = DateTime.UtcNow.AddDays(30),
            FechaPublicacion = DateTime.UtcNow.AddDays(-5),
            FechaCreacion = DateTime.UtcNow.AddDays(-10),
            Borrado = false,
            Rewards = new List<CampaniaCrowdfundingReward>(),
            Pedidos = new List<PedidoCrowdfunding>()
        };
    }

    public static CampaniaCrowdfundingReward CreateActiveReward(
        Guid? rewardId = null,
        Guid? campaniaId = null,
        decimal importeMinimo = 10m,
        int? cantidadMaxima = null)
    {
        return new CampaniaCrowdfundingReward
        {
            Id = new CampaniaCrowdfundingRewardId(rewardId ?? Guid.NewGuid()),
            CampaniaId = new CampaniaCrowdfundingId(campaniaId ?? Guid.NewGuid()),
            TipoRewardId = 1,
            Nombre = "CD Firmado",
            Descripcion = "CD con firma del artista",
            ImporteMinimo = importeMinimo,
            MonedaId = 1,
            EsAddOn = false,
            IncluyeEnvioFisico = false,
            CantidadMaxima = cantidadMaxima,
            Orden = 1,
            EsActivo = true,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public static PedidoCrowdfunding CreateCompletedPedido(
        Guid? pedidoId = null,
        Guid? campaniaId = null,
        decimal monto = 25m,
        string? userId = "user-123")
    {
        var pId = pedidoId ?? Guid.NewGuid();
        var cId = campaniaId ?? Guid.NewGuid();

        return new PedidoCrowdfunding
        {
            Id = new PedidoCrowdfundingId(pId),
            CampaniaId = new CampaniaCrowdfundingId(cId),
            UserId = userId,
            EstadoPedidoId = 3, // Completado
            MonedaId = 1,
            ImporteSubtotal = monto,
            ImportePropina = 0,
            ImporteEnvio = 0,
            ImporteImpuestos = 0,
            ImporteTotal = monto,
            PermitirMostrarNombre = true,
            ComentarioBacker = "Test message",
            FechaCreacion = DateTime.UtcNow,
            Lineas = new List<PedidoCrowdfundingLinea>(),
            Aportaciones = new List<AportacionCrowdfunding>()
        };
    }
}
