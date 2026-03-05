using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdpromotion.Domain.Model;

/// <summary>
/// Represents a promoter profile (fan/influencer who promotes campaigns).
/// </summary>
public class Promotor
{
    public PromotorId Id { get; set; }

    /// <summary>
    /// Reference to MaestraTipoPromotor
    /// </summary>
    public int TipoPromotorId { get; set; }

    /// <summary>
    /// Reference to Identity User
    /// </summary>
    public string UserId { get; set; } = null!;

    public FanProfileId? FanProfileId { get; set; }

    public string NombrePublico { get; set; } = null!;

    public string? EmailContacto { get; set; }

    public string? UrlSitioWeb { get; set; }

    public string? UrlInstagram { get; set; }

    public string? UrlTikTok { get; set; }

    public string? UrlTwitter { get; set; }

    public string? UrlYouTube { get; set; }

    public int? SeguidoresTotales { get; set; }

    public bool EsActivo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual ICollection<PromoProgramaPromotor> Programas { get; set; } = new List<PromoProgramaPromotor>();
    public virtual ICollection<PromotorWallet> Wallets { get; set; } = new List<PromotorWallet>();
}
