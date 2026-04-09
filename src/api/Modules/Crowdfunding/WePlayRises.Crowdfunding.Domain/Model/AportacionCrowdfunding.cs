using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents a payment/contribution for a crowdfunding order.
/// </summary>
public class AportacionCrowdfunding
{
    public AportacionCrowdfundingId Id { get; set; }

    public PedidoCrowdfundingId PedidoCrowdfundingId { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int MonedaId { get; set; }

    /// <summary>
    /// Reference to MaestraMetodoPago
    /// </summary>
    public int MetodoPagoId { get; set; }

    /// <summary>
    /// Reference to MaestraEstadoAportacionCrowd
    /// </summary>
    public int EstadoAportacionId { get; set; }

    public decimal ImporteTotal { get; set; }

    public decimal ImporteImpuestos { get; set; }

    public decimal ImporteComisionPlataforma { get; set; }

    public decimal ImporteComisionPasarela { get; set; }

    public decimal ImporteNetoArtista { get; set; }

    public string? CodigoOperacionPasarela { get; set; }

    public string? CodigoOperacionProveedor { get; set; }

    public DateTime? FechaAutorizacion { get; set; }

    public DateTime? FechaCaptura { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual PedidoCrowdfunding PedidoCrowdfunding { get; set; } = null!;
}
