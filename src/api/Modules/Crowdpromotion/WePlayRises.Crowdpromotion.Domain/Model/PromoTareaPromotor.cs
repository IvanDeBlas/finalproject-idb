namespace WePlayRises.Crowdpromotion.Domain.Model;

/// <summary>
/// Represents a promoter's completion of a task.
/// </summary>
public class PromoTareaPromotor
{
    public Guid Id { get; set; }

    public Guid TareaId { get; set; }

    public Guid ProgramaPromotorId { get; set; }

    /// <summary>
    /// Reference to MaestraEstadoTareaPromo
    /// </summary>
    public int EstadoTareaId { get; set; }

    public string? UrlPruebaCompletado { get; set; }

    public string? ComentarioPromotor { get; set; }

    public string? ComentarioValidacion { get; set; }

    public DateTime? FechaCompletado { get; set; }

    public DateTime? FechaValidado { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual PromoTarea Tarea { get; set; } = null!;
    public virtual PromoProgramaPromotor ProgramaPromotor { get; set; } = null!;
}
