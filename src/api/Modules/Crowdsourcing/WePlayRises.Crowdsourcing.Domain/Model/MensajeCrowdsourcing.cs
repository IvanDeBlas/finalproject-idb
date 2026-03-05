namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Represents a message in a crowdsourcing conversation.
/// </summary>
public class MensajeCrowdsourcing
{
    public Guid Id { get; set; }

    public Guid ConversacionId { get; set; }

    /// <summary>
    /// Reference to Identity User (sender)
    /// </summary>
    public string UserIdRemitente { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public string? UrlAdjunto { get; set; }

    public bool Leido { get; set; }

    public DateTime? FechaLeido { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual ConversacionCrowdsourcing Conversacion { get; set; } = null!;
}
