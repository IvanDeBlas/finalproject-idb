namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromotorMetricasResponseDto
{
    public Guid PromotorId { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public Guid? ProgramaId { get; set; }
    public string? ProgramaTitulo { get; set; }
    public string FechaDesde { get; set; } = null!;
    public string FechaHasta { get; set; } = null!;
    public PromotorMetricasKpisDto Kpis { get; set; } = null!;
    public IReadOnlyList<EventoRecienteDto> EventosRecientes { get; set; } = Array.Empty<EventoRecienteDto>();
}
