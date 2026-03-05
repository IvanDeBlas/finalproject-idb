namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class CerrarNecesidadResultDto
{
    public Guid Id { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public int PropuestasRechazadas { get; set; }
}
