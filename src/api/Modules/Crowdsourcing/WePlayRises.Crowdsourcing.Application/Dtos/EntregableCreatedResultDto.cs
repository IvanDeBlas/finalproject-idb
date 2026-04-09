namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class EntregableCreatedResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string EstadoEntregableNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
