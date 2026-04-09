namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class AceptarPropuestaResultDto
{
    public Guid AcuerdoId { get; set; }
    public string TituloInterno { get; set; } = null!;
    public string EstadoAcuerdoNombre { get; set; } = null!;
    public decimal ImporteTotalPactado { get; set; }
    public string MonedaNombre { get; set; } = null!;
    public Guid ConversacionId { get; set; }
    public int PropuestasRechazadas { get; set; }
}
