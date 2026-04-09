using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.UserAccess.Domain.Model;

/// <summary>
/// Represents a portfolio item for a professional profile.
/// </summary>
public class PerfilProfesionalPortfolioItem
{
    public Guid Id { get; set; }

    public PerfilProfesionalId PerfilProfesionalId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? UrlRecurso { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual PerfilProfesional PerfilProfesional { get; set; } = null!;
}
