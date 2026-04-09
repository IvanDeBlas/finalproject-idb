namespace WePlayRises.Crowdpromotion.Domain.Model;

/// <summary>
/// Represents a transaction in a promoter's wallet.
/// </summary>
public class PromotorWalletTransaccion
{
    public Guid Id { get; set; }

    public Guid WalletId { get; set; }

    /// <summary>
    /// Reference to MaestraTipoReward (type of earning)
    /// </summary>
    public int? TipoRewardId { get; set; }

    /// <summary>
    /// Reference to MaestraEstadoWalletTransaccion
    /// </summary>
    public int EstadoTransaccionId { get; set; }

    public Guid? CampaniaPayoutId { get; set; }

    public decimal Importe { get; set; }

    public string? Concepto { get; set; }

    public string? ReferenciaExterna { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaProcesado { get; set; }

    /// <summary>
    /// true = credito/ingreso (comision acreditada), false = debito/retiro.
    /// </summary>
    public bool EsCredito { get; set; }

    /// <summary>
    /// FK to PromoEvento that originated this credit. Null for withdrawals (debits).
    /// </summary>
    public Guid? PromoEventoId { get; set; }

    /// <summary>
    /// Human-readable description of the transaction origin. Max 500 chars.
    /// </summary>
    public string? Descripcion { get; set; }

    // Navigation properties
    public virtual PromotorWallet Wallet { get; set; } = null!;
    public virtual PromoEvento? PromoEvento { get; set; }
}
