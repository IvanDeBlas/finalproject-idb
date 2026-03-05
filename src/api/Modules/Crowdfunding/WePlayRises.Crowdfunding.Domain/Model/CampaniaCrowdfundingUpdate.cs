using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents an update/progress post for a crowdfunding campaign.
/// </summary>
public class CampaniaCrowdfundingUpdate
{
    public Guid Id { get; set; }

    public CampaniaCrowdfundingId CampaniaId { get; set; }

    public string Titulo { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public bool EsPublico { get; set; }

    public bool SoloBackers { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual CampaniaCrowdfunding Campania { get; set; } = null!;
}
