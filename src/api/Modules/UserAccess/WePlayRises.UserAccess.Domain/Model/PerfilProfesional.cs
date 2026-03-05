using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.UserAccess.Domain.Model;

/// <summary>
/// Represents a professional profile for service providers (freelancers).
/// </summary>
public class PerfilProfesional
{
    public PerfilProfesionalId Id { get; set; }

    /// <summary>
    /// Reference to the Identity User
    /// </summary>
    public string UserId { get; set; } = null!;

    public string? Titulo { get; set; }

    public string? Descripcion { get; set; }

    public decimal? TarifaHora { get; set; }

    public decimal? TarifaProyectoMin { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int? MonedaId { get; set; }

    public string? UrlPortfolio { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual ICollection<PerfilProfesionalSkill> Skills { get; set; } = new List<PerfilProfesionalSkill>();
    public virtual ICollection<PerfilProfesionalPortfolioItem> PortfolioItems { get; set; } = new List<PerfilProfesionalPortfolioItem>();
}
