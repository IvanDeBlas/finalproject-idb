using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents a user's subscription to an artist's membership plan.
/// </summary>
public class ArtistaMembershipSuscripcion
{
    public Guid Id { get; set; }

    public ArtistaId ArtistaId { get; set; }

    public Guid MembershipPlanId { get; set; }

    /// <summary>
    /// Reference to Identity User
    /// </summary>
    public string UserId { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public DateTime? ProximoCargo { get; set; }

    public bool EsActiva { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual ArtistaMembershipPlan MembershipPlan { get; set; } = null!;
    public virtual ICollection<ArtistaMembershipPago> Pagos { get; set; } = new List<ArtistaMembershipPago>();
}
