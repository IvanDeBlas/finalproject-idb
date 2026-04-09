using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Represents a rating/review for a crowdsourcing agreement.
/// </summary>
public class ValoracionCrowdsourcing
{
    public Guid Id { get; set; }

    public AcuerdoCrowdsourcingId AcuerdoId { get; set; }

    /// <summary>
    /// Reference to Identity User (author of rating)
    /// </summary>
    public string UserIdAutor { get; set; } = null!;

    /// <summary>
    /// Reference to Identity User (target of rating)
    /// </summary>
    public string UserIdValorado { get; set; } = null!;

    /// <summary>
    /// Reference to MaestraTipoValoracion
    /// </summary>
    public int? TipoValoracionId { get; set; }

    /// <summary>
    /// Rating from 1 to 5
    /// </summary>
    public byte Puntuacion { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual AcuerdoCrowdsourcing Acuerdo { get; set; } = null!;
}
