using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Represents a service agreement between an artist and a provider.
/// </summary>
public class AcuerdoCrowdsourcing
{
    public AcuerdoCrowdsourcingId Id { get; set; }

    public NecesidadCrowdsourcingId NecesidadId { get; set; }

    public PropuestaCrowdsourcingId? PropuestaId { get; set; }

    public ArtistaId ArtistaId { get; set; }

    /// <summary>
    /// Reference to Identity User (provider)
    /// </summary>
    public string UserIdProveedor { get; set; } = null!;

    public PerfilProfesionalId? PerfilProfesionalId { get; set; }

    public string? TituloInterno { get; set; }

    public string? Descripcion { get; set; }

    /// <summary>
    /// Reference to MaestraEstadoAcuerdo
    /// </summary>
    public int EstadoAcuerdoId { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int? MonedaId { get; set; }

    public decimal ImporteTotalPactado { get; set; }

    public decimal? ImporteAnticipo { get; set; }

    public decimal? PorcentajeAnticipo { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFinPrevista { get; set; }

    public DateTime? FechaFinReal { get; set; }

    public string? MotivoCancelacion { get; set; }

    public string? CanceladoPor { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual NecesidadCrowdsourcing Necesidad { get; set; } = null!;
    public virtual PropuestaCrowdsourcing? Propuesta { get; set; }
    public virtual ICollection<AcuerdoCrowdsourcingMilestone> Milestones { get; set; } = new List<AcuerdoCrowdsourcingMilestone>();
    public virtual ICollection<AcuerdoCrowdsourcingEntregable> Entregables { get; set; } = new List<AcuerdoCrowdsourcingEntregable>();
    public virtual ICollection<ConversacionCrowdsourcing> Conversaciones { get; set; } = new List<ConversacionCrowdsourcing>();
    public virtual ICollection<ValoracionCrowdsourcing> Valoraciones { get; set; } = new List<ValoracionCrowdsourcing>();
}
