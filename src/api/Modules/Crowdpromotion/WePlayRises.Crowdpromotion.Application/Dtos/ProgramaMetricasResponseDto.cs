namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class ProgramaMetricasResponseDto
{
    public Guid ProgramaId { get; set; }
    public string ProgramaTitulo { get; set; } = null!;
    public string FechaDesde { get; set; } = null!;
    public string FechaHasta { get; set; } = null!;
    public ProgramaMetricasKpisDto Kpis { get; set; } = null!;
    public IReadOnlyList<RankingPromotorItemDto> RankingPromotores { get; set; } = Array.Empty<RankingPromotorItemDto>();
    public IReadOnlyList<EventosPorDiaItemDto> EventosPorDia { get; set; } = Array.Empty<EventosPorDiaItemDto>();
}
