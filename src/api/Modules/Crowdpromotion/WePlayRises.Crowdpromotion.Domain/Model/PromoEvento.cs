using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdpromotion.Domain.Model;

/// <summary>
/// Represents a promotional event/action tracked for analytics.
/// </summary>
public class PromoEvento
{
    public Guid Id { get; set; }

    public PromoProgramaId? ProgramaId { get; set; }

    public PromotorId? PromotorId { get; set; }

    public CampaniaCrowdfundingId? CampaniaCrowdfundingId { get; set; }

    public PedidoCrowdfundingId? PedidoCrowdfundingId { get; set; }

    public AportacionCrowdfundingId? AportacionCrowdfundingId { get; set; }

    /// <summary>
    /// Reference to MaestraTipoEventoPromo
    /// </summary>
    public int TipoEventoId { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int? MonedaId { get; set; }

    public decimal? ImporteAsociado { get; set; }

    public string? CodigoReferido { get; set; }

    public string? IpOrigen { get; set; }

    public string? UserAgentOrigen { get; set; }

    /// <summary>
    /// FK to PromoProgramaPromotor (inscription join table). Null if anonymous or invalid referral code.
    /// </summary>
    public Guid? PromoProgramaPromotorId { get; set; }

    /// <summary>
    /// UserId of the fan who performed the action (from JWT if authenticated).
    /// </summary>
    public string? UserIdAfectado { get; set; }

    /// <summary>
    /// Full URL the fan came from.
    /// </summary>
    public string? UrlOrigen { get; set; }

    /// <summary>
    /// HTTP Referer header.
    /// </summary>
    public string? UrlReferer { get; set; }

    /// <summary>
    /// UTM source parameter (always "weplay" from frontend).
    /// </summary>
    public string? UtmSource { get; set; }

    /// <summary>
    /// UTM medium parameter ("referral").
    /// </summary>
    public string? UtmMedium { get; set; }

    /// <summary>
    /// UTM campaign parameter (matches CodigoTrackingBase of the program).
    /// </summary>
    public string? UtmCampaign { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual PromoPrograma? Programa { get; set; }
    public virtual Promotor? Promotor { get; set; }
}
