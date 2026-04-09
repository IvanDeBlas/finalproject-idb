using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdpromotion.Domain.Model;

/// <summary>
/// Represents a promoter's wallet/balance in a specific currency.
/// </summary>
public class PromotorWallet
{
    public Guid Id { get; set; }

    public PromotorId PromotorId { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int MonedaId { get; set; }

    public decimal SaldoDisponible { get; set; }

    public decimal SaldoPendiente { get; set; }

    public decimal TotalGanado { get; set; }

    public decimal TotalRetirado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    /// <summary>
    /// Optimistic concurrency token. EF Core manages automatically via IsRowVersion().
    /// Prevents double-debit on concurrent withdrawal requests (RN-05).
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    // Navigation properties
    public virtual Promotor Promotor { get; set; } = null!;
    public virtual ICollection<PromotorWalletTransaccion> Transacciones { get; set; } = new List<PromotorWalletTransaccion>();
}
