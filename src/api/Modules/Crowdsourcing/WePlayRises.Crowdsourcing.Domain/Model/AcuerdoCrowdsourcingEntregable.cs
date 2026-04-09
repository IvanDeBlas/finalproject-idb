using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Represents a deliverable in a crowdsourcing agreement.
/// </summary>
public class AcuerdoCrowdsourcingEntregable
{
    public Guid Id { get; set; }

    public AcuerdoCrowdsourcingId AcuerdoId { get; set; }

    public Guid? MilestoneId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? UrlRecurso { get; set; }

    /// <summary>
    /// Reference to MaestraEstadoEntregable
    /// </summary>
    public int EstadoEntregableId { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public DateTime? FechaAprobacion { get; set; }

    public string? ComentarioAprobacion { get; set; }

    public string? ComentarioRechazo { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual AcuerdoCrowdsourcing Acuerdo { get; set; } = null!;
    public virtual AcuerdoCrowdsourcingMilestone? Milestone { get; set; }
}
