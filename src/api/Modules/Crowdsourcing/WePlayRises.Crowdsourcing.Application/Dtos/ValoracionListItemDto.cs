namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ValoracionListItemDto
{
    public Guid Id { get; set; }
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
    public string AutorNombre { get; set; } = null!;
    public string? AutorImagenUrl { get; set; }
    public string AcuerdoTituloInterno { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
