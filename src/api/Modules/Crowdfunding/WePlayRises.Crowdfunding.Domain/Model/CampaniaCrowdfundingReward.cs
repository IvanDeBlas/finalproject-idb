using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents a reward tier for a crowdfunding campaign.
/// </summary>
public class CampaniaCrowdfundingReward
{
    public CampaniaCrowdfundingRewardId Id { get; set; }

    public CampaniaCrowdfundingId CampaniaId { get; set; }

    /// <summary>
    /// Reference to MaestraTipoReward
    /// </summary>
    public int TipoRewardId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal ImporteMinimo { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int MonedaId { get; set; }

    public bool EsAddOn { get; set; }

    public int? CantidadMaxima { get; set; }

    public int? CantidadPorBacker { get; set; }

    public bool IncluyeEnvioFisico { get; set; }

    public string? TiempoEntregaEstimado { get; set; }

    public int Orden { get; set; }

    public bool EsActivo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual CampaniaCrowdfunding Campania { get; set; } = null!;
    public virtual ICollection<PedidoCrowdfundingLinea> Lineas { get; set; } = new List<PedidoCrowdfundingLinea>();
}
