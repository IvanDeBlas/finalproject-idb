using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents a backer's order/pledge in a crowdfunding campaign.
/// </summary>
public class PedidoCrowdfunding
{
    public PedidoCrowdfundingId Id { get; set; }

    public CampaniaCrowdfundingId CampaniaId { get; set; }

    /// <summary>
    /// Reference to Identity User (optional for anonymous)
    /// </summary>
    public string? UserId { get; set; }

    public FanProfileId? FanProfileId { get; set; }

    /// <summary>
    /// Reference to MaestraEstadoPedidoCrowd
    /// </summary>
    public int EstadoPedidoId { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int MonedaId { get; set; }

    public decimal ImporteSubtotal { get; set; }

    public decimal ImportePropina { get; set; }

    public decimal ImporteEnvio { get; set; }

    public decimal ImporteImpuestos { get; set; }

    public decimal ImporteTotal { get; set; }

    public bool PermitirMostrarNombre { get; set; }

    public string? ComentarioBacker { get; set; }

    /// <summary>
    /// Reference to DireccionPostal for shipping
    /// </summary>
    public Guid? DireccionEnvioId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual CampaniaCrowdfunding Campania { get; set; } = null!;
    public virtual ICollection<PedidoCrowdfundingLinea> Lineas { get; set; } = new List<PedidoCrowdfundingLinea>();
    public virtual ICollection<AportacionCrowdfunding> Aportaciones { get; set; } = new List<AportacionCrowdfunding>();
}
