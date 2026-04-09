using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdpromotion.Domain.Model;

/// <summary>
/// Represents the assignment of a promoter to a promotional program.
/// </summary>
public class PromoProgramaPromotor
{
    public Guid Id { get; set; }

    public PromoProgramaId ProgramaId { get; set; }

    public PromotorId PromotorId { get; set; }

    public string? CodigoReferido { get; set; }

    public string? UrlReferido { get; set; }

    public int TotalClicks { get; set; }

    public int TotalConversiones { get; set; }

    public decimal TotalComisionesGeneradas { get; set; }

    public bool EsActivo { get; set; }

    public bool EsAprobado { get; set; }

    public bool EsBloqueado { get; set; }

    public DateTime FechaInscripcion { get; set; }

    public DateTime? FechaBaja { get; set; }

    // Navigation properties
    public virtual PromoPrograma Programa { get; set; } = null!;
    public virtual Promotor Promotor { get; set; } = null!;
    public virtual ICollection<PromoTareaPromotor> TareasAsignadas { get; set; } = new List<PromoTareaPromotor>();
}
