namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class EntregableDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? UrlRecurso { get; set; }
    public int EstadoEntregableId { get; set; }
    public string EstadoEntregableNombre { get; set; } = null!;
    public string? ComentarioAprobacion { get; set; }
    public string? ComentarioRechazo { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public DateTime FechaCreacion { get; set; }
}
