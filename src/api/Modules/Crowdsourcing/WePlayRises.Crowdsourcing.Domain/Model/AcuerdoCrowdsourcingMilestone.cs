using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Represents a milestone/phase in a crowdsourcing agreement.
/// </summary>
public class AcuerdoCrowdsourcingMilestone
{
    public Guid Id { get; set; }

    public AcuerdoCrowdsourcingId AcuerdoId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int Orden { get; set; }

    public decimal ImporteParcial { get; set; }

    public decimal? PorcentajeParcial { get; set; }

    public DateTime? FechaLimite { get; set; }

    public DateTime? FechaCompletado { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual AcuerdoCrowdsourcing Acuerdo { get; set; } = null!;
    public virtual ICollection<AcuerdoCrowdsourcingEntregable> Entregables { get; set; } = new List<AcuerdoCrowdsourcingEntregable>();
}
