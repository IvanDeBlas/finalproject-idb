namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class CompletarAcuerdoResultDto
{
    public Guid Id { get; set; }
    public string EstadoAcuerdoNombre { get; set; } = null!;
    public DateTime FechaFinReal { get; set; }
}
