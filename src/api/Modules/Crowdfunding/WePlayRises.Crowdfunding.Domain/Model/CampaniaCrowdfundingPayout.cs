using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents a payout to an artist from a crowdfunding campaign.
/// </summary>
public class CampaniaCrowdfundingPayout
{
    public Guid Id { get; set; }

    public CampaniaCrowdfundingId CampaniaId { get; set; }

    public Guid ArtistaPayoutCuentaId { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int MonedaId { get; set; }

    public decimal ImporteBruto { get; set; }

    public decimal ImporteComisionPlataforma { get; set; }

    public decimal ImporteComisionPasarela { get; set; }

    public decimal ImporteImpuestosRetenidos { get; set; }

    public decimal ImporteNetoArtista { get; set; }

    /// <summary>
    /// Reference to MaestraEstadoPayoutCrowd
    /// </summary>
    public int EstadoPayoutId { get; set; }

    public DateTime? FechaProgramada { get; set; }

    public DateTime? FechaEjecucion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public string? NotasInternas { get; set; }

    // Navigation properties
    public virtual CampaniaCrowdfunding Campania { get; set; } = null!;
    public virtual ArtistaPayoutCuenta ArtistaPayoutCuenta { get; set; } = null!;
}
