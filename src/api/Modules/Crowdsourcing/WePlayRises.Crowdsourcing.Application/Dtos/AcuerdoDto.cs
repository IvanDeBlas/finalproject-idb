namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class AcuerdoDto
{
    public Guid Id { get; set; }
    public string TituloInterno { get; set; } = null!;
    public int EstadoAcuerdoId { get; set; }
    public string EstadoAcuerdoNombre { get; set; } = null!;
    public decimal ImporteTotalPactado { get; set; }
    public string MonedaNombre { get; set; } = null!;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFinPrevista { get; set; }
    public DateTime? FechaFinReal { get; set; }
    public AcuerdoArtistaDto Artista { get; set; } = null!;
    public AcuerdoProfesionalDto Profesional { get; set; } = null!;
    public AcuerdoNecesidadDto Necesidad { get; set; } = null!;
    public Guid? ConversacionId { get; set; }
    public List<MilestoneDto> Milestones { get; set; } = new();
    public decimal ImporteAsignado { get; set; }
    public decimal PorcentajeAsignado { get; set; }
    public string MiRol { get; set; } = null!;
    public List<AcuerdoTimelineEventoDto> Timeline { get; set; } = new();
}
