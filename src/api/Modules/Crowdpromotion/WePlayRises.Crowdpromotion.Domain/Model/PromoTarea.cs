using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdpromotion.Domain.Model;

/// <summary>
/// Represents a task/action within a promotional program.
/// </summary>
public class PromoTarea
{
    public Guid Id { get; set; }

    public PromoProgramaId ProgramaId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? InstruccionesUrl { get; set; }

    /// <summary>
    /// Reference to MaestraTipoReward (reward for completing)
    /// </summary>
    public int? TipoRewardId { get; set; }

    public decimal? ImporteRecompensa { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int? MonedaId { get; set; }

    public int? PuntosRecompensa { get; set; }

    /// <summary>
    /// Reference to MaestraTipoEventoPromo
    /// </summary>
    public int TipoEventoPromoId { get; set; }

    public bool EsRepetible { get; set; }

    public int? MaxRepeticiones { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int Orden { get; set; }

    public bool EsActivo { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual PromoPrograma Programa { get; set; } = null!;
    public virtual ICollection<PromoTareaPromotor> TareasPromotor { get; set; } = new List<PromoTareaPromotor>();
}
