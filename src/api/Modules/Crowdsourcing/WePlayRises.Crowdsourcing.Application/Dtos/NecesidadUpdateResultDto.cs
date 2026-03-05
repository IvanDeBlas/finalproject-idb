namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class NecesidadUpdateResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public DateTime? FechaActualizacion { get; set; }
}
