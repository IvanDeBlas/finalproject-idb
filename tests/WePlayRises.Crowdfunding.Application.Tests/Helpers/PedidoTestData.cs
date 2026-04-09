using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Commands;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Helpers;

public static class PedidoTestData
{
    public static CreatePedidoCommand CreateValidCommand(
        Guid? campaniaId = null,
        string? userId = "user-123",
        decimal importeTotal = 50m)
    {
        return new CreatePedidoCommand
        {
            CampaniaId = campaniaId ?? Guid.NewGuid(),
            UserId = userId,
            MonedaId = 1,
            ImporteSubtotal = importeTotal - 5m,
            ImportePropina = 5m,
            ImporteEnvio = 0m,
            ImporteImpuestos = 0m,
            ImporteTotal = importeTotal,
            PermitirMostrarNombre = true,
            ComentarioBacker = "Great project!"
        };
    }

    public static PedidoCrowdfunding CreateValid(
        Guid? pedidoId = null,
        Guid? campaniaId = null,
        string? userId = "user-123",
        decimal importeTotal = 50m,
        int estadoPedidoId = 1)
    {
        var pId = pedidoId ?? Guid.NewGuid();
        var cId = campaniaId ?? Guid.NewGuid();

        return new PedidoCrowdfunding
        {
            Id = new PedidoCrowdfundingId(pId),
            CampaniaId = new CampaniaCrowdfundingId(cId),
            UserId = userId,
            EstadoPedidoId = estadoPedidoId,
            MonedaId = 1,
            ImporteSubtotal = importeTotal - 5m,
            ImportePropina = 5m,
            ImporteEnvio = 0m,
            ImporteImpuestos = 0m,
            ImporteTotal = importeTotal,
            PermitirMostrarNombre = true,
            ComentarioBacker = "Great project!",
            FechaCreacion = DateTime.UtcNow,
            Lineas = new List<PedidoCrowdfundingLinea>(),
            Aportaciones = new List<AportacionCrowdfunding>()
        };
    }

    public static UpdatePedidoCommand CreateValidUpdateCommand(
        Guid? pedidoId = null)
    {
        return new UpdatePedidoCommand
        {
            Id = pedidoId ?? Guid.NewGuid(),
            EstadoPedidoId = 2,
            ImportePropina = 10m,
            ComentarioBacker = "Updated comment"
        };
    }
}
