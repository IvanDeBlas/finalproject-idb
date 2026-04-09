namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ConversacionListItemDto
{
    public Guid Id { get; set; }
    public string Asunto { get; set; } = null!;
    public string NombreOtraParte { get; set; } = null!;
    public string? ImagenOtraParte { get; set; }
    public string ContextoTipo { get; set; } = null!;
    public string ContextoTitulo { get; set; } = null!;
    public string? UltimoMensaje { get; set; }
    public DateTime? FechaUltimoMensaje { get; set; }
    public int MensajesNoLeidos { get; set; }
}
