using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Tests.Helpers;

public static class RewardTestData
{
    public static CampaniaCrowdfundingReward CreateValid(
        Guid? rewardId = null,
        Guid? campaniaId = null,
        string nombre = "Test Reward",
        decimal importeMinimo = 25m,
        int orden = 1,
        bool esActivo = true)
    {
        var rId = rewardId ?? Guid.NewGuid();
        var cId = campaniaId ?? Guid.NewGuid();

        return new CampaniaCrowdfundingReward
        {
            Id = new CampaniaCrowdfundingRewardId(rId),
            CampaniaId = new CampaniaCrowdfundingId(cId),
            TipoRewardId = 1,
            Nombre = nombre,
            Descripcion = "Test description",
            ImporteMinimo = importeMinimo,
            MonedaId = 1,
            EsAddOn = false,
            IncluyeEnvioFisico = false,
            Orden = orden,
            EsActivo = esActivo,
            FechaCreacion = DateTime.UtcNow,
        };
    }

    public static CampaniaCrowdfundingReward CreateWithLineas(
        Guid? rewardId = null,
        Guid? campaniaId = null,
        int lineaCount = 1)
    {
        var reward = CreateValid(rewardId, campaniaId);
        reward.Lineas = Enumerable.Range(0, lineaCount)
            .Select(_ => new PedidoCrowdfundingLinea
            {
                Id = Guid.NewGuid(),
                RewardId = reward.Id,
                Cantidad = 1,
                PrecioUnitario = reward.ImporteMinimo,
                ImporteLinea = reward.ImporteMinimo,
                FechaCreacion = DateTime.UtcNow,
            })
            .ToList();
        return reward;
    }
}
