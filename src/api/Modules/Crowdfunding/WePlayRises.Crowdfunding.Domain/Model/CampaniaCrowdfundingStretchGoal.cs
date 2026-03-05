using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents a stretch goal for a crowdfunding campaign.
/// </summary>
public class CampaniaCrowdfundingStretchGoal
{
    public Guid Id { get; set; }

    public CampaniaCrowdfundingId CampaniaId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal ImporteObjetivo { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int MonedaId { get; set; }

    public int Orden { get; set; }

    public bool Alcanzado { get; set; }

    public DateTime? FechaAlcanzado { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual CampaniaCrowdfunding Campania { get; set; } = null!;
}
