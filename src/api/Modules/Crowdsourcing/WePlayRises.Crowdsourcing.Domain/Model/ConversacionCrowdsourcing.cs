using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Represents a conversation thread between artist and provider.
/// </summary>
public class ConversacionCrowdsourcing
{
    public Guid Id { get; set; }

    public NecesidadCrowdsourcingId? NecesidadId { get; set; }

    public AcuerdoCrowdsourcingId? AcuerdoId { get; set; }

    /// <summary>
    /// Reference to Identity User (creator of the conversation)
    /// </summary>
    public string UserIdCreador { get; set; } = null!;

    /// <summary>
    /// Reference to Identity User (recipient of the conversation)
    /// </summary>
    public string UserIdDestinatario { get; set; } = null!;

    public string Asunto { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaUltimoMensaje { get; set; }

    // Navigation properties
    public virtual NecesidadCrowdsourcing? Necesidad { get; set; }
    public virtual AcuerdoCrowdsourcing? Acuerdo { get; set; }
    public virtual ICollection<MensajeCrowdsourcing> Mensajes { get; set; } = new List<MensajeCrowdsourcing>();
}
