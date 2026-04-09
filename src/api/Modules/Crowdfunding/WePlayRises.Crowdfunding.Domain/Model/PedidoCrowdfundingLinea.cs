using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents a line item in a crowdfunding order.
/// </summary>
public class PedidoCrowdfundingLinea
{
    public Guid Id { get; set; }

    public PedidoCrowdfundingId PedidoCrowdfundingId { get; set; }

    public CampaniaCrowdfundingRewardId RewardId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal ImporteLinea { get; set; }

    public bool EsRewardPrincipal { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual PedidoCrowdfunding PedidoCrowdfunding { get; set; } = null!;
    public virtual CampaniaCrowdfundingReward Reward { get; set; } = null!;
}
