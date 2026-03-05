using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdpromotion.Domain.Model;

/// <summary>
/// Represents a promotional program/campaign created by an artist.
/// </summary>
public class PromoPrograma
{
    public PromoProgramaId Id { get; set; }

    public ArtistaId ArtistaId { get; set; }

    public ProyectoArtisticoId? ProyectoArtisticoId { get; set; }

    public CampaniaCrowdfundingId? CampaniaCrowdfundingId { get; set; }

    /// <summary>
    /// Reference to MaestraTipoPromo
    /// </summary>
    public int TipoPromoId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int? MonedaId { get; set; }

    public string? UrlLanding { get; set; }

    public string? CodigoTrackingBase { get; set; }

    public decimal? ImporteComisionPorcentaje { get; set; }

    public decimal? ImporteComisionFija { get; set; }

    public decimal? PresupuestoTotal { get; set; }

    public decimal? ComisionPorConversion { get; set; }

    public decimal? ComisionPorClick { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public bool EsActivo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual ICollection<PromoProgramaPromotor> Promotores { get; set; } = new List<PromoProgramaPromotor>();
    public virtual ICollection<PromoTarea> Tareas { get; set; } = new List<PromoTarea>();
    public virtual ICollection<PromoEvento> Eventos { get; set; } = new List<PromoEvento>();
}
