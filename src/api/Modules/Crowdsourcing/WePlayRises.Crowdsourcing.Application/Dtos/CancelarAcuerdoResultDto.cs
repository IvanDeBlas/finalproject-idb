namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class CancelarAcuerdoResultDto
{
    public Guid Id { get; set; }
    public string EstadoAcuerdoNombre { get; set; } = null!;
    public DateTime FechaFinReal { get; set; }
    public string NecesidadEstadoNombre { get; set; } = null!;
}
