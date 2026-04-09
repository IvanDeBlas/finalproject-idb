using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents a comment on a crowdfunding campaign.
/// </summary>
public class CampaniaCrowdfundingComentario
{
    public Guid Id { get; set; }

    public CampaniaCrowdfundingId CampaniaId { get; set; }

    /// <summary>
    /// Reference to Identity User
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Parent comment ID for threading
    /// </summary>
    public Guid? ComentarioPadreId { get; set; }

    public string Contenido { get; set; } = null!;

    public bool EsRespuestaArtista { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public bool Borrado { get; set; }

    // Navigation properties
    public virtual CampaniaCrowdfunding Campania { get; set; } = null!;
    public virtual CampaniaCrowdfundingComentario? ComentarioPadre { get; set; }
    public virtual ICollection<CampaniaCrowdfundingComentario> Respuestas { get; set; } = new List<CampaniaCrowdfundingComentario>();
}
