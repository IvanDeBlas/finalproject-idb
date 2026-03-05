using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents a membership/subscription tier offered by an artist.
/// </summary>
public class ArtistaMembershipPlan
{
    public Guid Id { get; set; }

    public ArtistaId ArtistaId { get; set; }

    public string NombrePlan { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal ImporteMensual { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int MonedaId { get; set; }

    /// <summary>
    /// Tier level (1 = basic, higher = more benefits)
    /// </summary>
    public int Nivel { get; set; }

    public bool EsActivo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual ICollection<ArtistaMembershipSuscripcion> Suscripciones { get; set; } = new List<ArtistaMembershipSuscripcion>();
}
