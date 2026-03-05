using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Represents a proposal/bid from a service provider for a need.
/// </summary>
public class PropuestaCrowdsourcing
{
    public PropuestaCrowdsourcingId Id { get; set; }

    public NecesidadCrowdsourcingId NecesidadId { get; set; }

    /// <summary>
    /// Reference to Identity User (provider)
    /// </summary>
    public string UserId { get; set; } = null!;

    public PerfilProfesionalId? PerfilProfesionalId { get; set; }

    public string? MensajePropuesta { get; set; }

    public decimal PrecioPropuesto { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int? MonedaId { get; set; }

    public int? DiasEstimados { get; set; }

    /// <summary>
    /// Reference to MaestraEstadoPropuesta
    /// </summary>
    public int EstadoPropuestaId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public string? MotivoRechazo { get; set; }

    public Guid? AcuerdoId { get; set; }

    // Navigation properties
    public virtual NecesidadCrowdsourcing Necesidad { get; set; } = null!;
    public virtual ICollection<AcuerdoCrowdsourcing> Acuerdos { get; set; } = new List<AcuerdoCrowdsourcing>();
}
