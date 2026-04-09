namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class MensajeDto
{
    public Guid Id { get; set; }
    public string Contenido { get; set; } = null!;
    public string? UrlAdjunto { get; set; }
    public string RemitenteNombre { get; set; } = null!;
    public bool EsPropio { get; set; }
    public bool Leido { get; set; }
    public DateTime FechaCreacion { get; set; }
}
