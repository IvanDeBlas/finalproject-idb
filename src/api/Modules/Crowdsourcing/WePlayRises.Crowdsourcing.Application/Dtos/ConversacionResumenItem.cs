namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ConversacionResumenItem
{
    public Guid Id { get; set; }
    public string UserIdCreador { get; set; } = null!;
    public string UserIdDestinatario { get; set; } = null!;
    public string Asunto { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaUltimoMensaje { get; set; }
    public Guid? NecesidadId { get; set; }
    public Guid? AcuerdoId { get; set; }
    public string ContextoTitulo { get; set; } = string.Empty;
    public string? UltimoMensajePreview { get; set; }
    public int MensajesNoLeidos { get; set; }
}
