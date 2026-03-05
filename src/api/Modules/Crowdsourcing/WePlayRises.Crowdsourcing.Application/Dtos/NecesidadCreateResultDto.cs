namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class NecesidadCreateResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
