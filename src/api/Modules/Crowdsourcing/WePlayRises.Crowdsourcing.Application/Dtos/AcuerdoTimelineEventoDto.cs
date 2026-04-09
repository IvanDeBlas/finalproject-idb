namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class AcuerdoTimelineEventoDto
{
    public string Accion { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public string Actor { get; set; } = null!;
}
