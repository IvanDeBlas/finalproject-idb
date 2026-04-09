using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdfunding.Domain.Model;

/// <summary>
/// Represents a crowdfunding campaign.
/// </summary>
public class CampaniaCrowdfunding
{
    public CampaniaCrowdfundingId Id { get; set; }

    public ArtistaId ArtistaId { get; set; }

    public ProyectoArtisticoId? ProyectoArtisticoId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Subtitulo { get; set; }

    public string? DescripcionCorta { get; set; }

    public string? VideoPrincipalUrl { get; set; }

    public string? ImagenPrincipalUrl { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int MonedaId { get; set; }

    public decimal ImporteObjetivo { get; set; }

    public decimal? ImporteMinimo { get; set; }

    public decimal ImportePledgedActual { get; set; }

    /// <summary>
    /// Reference to MaestraTipoFinanciacion
    /// </summary>
    public int TipoFinanciacionId { get; set; }

    /// <summary>
    /// Reference to MaestraEstadoCampaniaCrowd
    /// </summary>
    public int EstadoCampaniaId { get; set; }

    public bool PermiteAportacionesAnonimas { get; set; }

    public bool PermitePropinas { get; set; }

    public decimal? PorcentajeComisionPlataforma { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public DateTime? FechaPublicacion { get; set; }

    public DateTime? FechaCierre { get; set; }

    public bool Borrado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual ICollection<CampaniaCrowdfundingReward> Rewards { get; set; } = new List<CampaniaCrowdfundingReward>();
    public virtual ICollection<PedidoCrowdfunding> Pedidos { get; set; } = new List<PedidoCrowdfunding>();
    public virtual ICollection<CampaniaCrowdfundingPayout> Payouts { get; set; } = new List<CampaniaCrowdfundingPayout>();
    public virtual ICollection<CampaniaCrowdfundingUpdate> Updates { get; set; } = new List<CampaniaCrowdfundingUpdate>();
    public virtual ICollection<CampaniaCrowdfundingComentario> Comentarios { get; set; } = new List<CampaniaCrowdfundingComentario>();
    public virtual ICollection<CampaniaCrowdfundingStretchGoal> StretchGoals { get; set; } = new List<CampaniaCrowdfundingStretchGoal>();
}
