namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class AprobarEntregableResultDto
{
    public Guid Id { get; set; }
    public string EstadoEntregableNombre { get; set; } = null!;
    public DateTime FechaAprobacion { get; set; }
    public bool TodosAprobadosEnMilestone { get; set; }
}
