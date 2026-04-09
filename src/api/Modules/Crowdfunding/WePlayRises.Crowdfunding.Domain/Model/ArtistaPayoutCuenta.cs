using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents an artist's payout/bank account for receiving funds.
/// </summary>
public class ArtistaPayoutCuenta
{
    public Guid Id { get; set; }

    public ArtistaId ArtistaId { get; set; }

    public string NombreCuenta { get; set; } = null!;

    public string TipoCuenta { get; set; } = null!;

    public string? ProveedorPayout { get; set; }

    public string? IdentificadorExterno { get; set; }

    public string? NombreTitular { get; set; }

    public string? Iban { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int? MonedaId { get; set; }

    public bool EsPorDefecto { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaBaja { get; set; }

    // Navigation properties
    public virtual ICollection<CampaniaCrowdfundingPayout> Payouts { get; set; } = new List<CampaniaCrowdfundingPayout>();
}
