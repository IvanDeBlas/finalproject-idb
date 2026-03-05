namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromoProgramaPromotorSummaryDto
{
    public Guid Id { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public string TipoPromotorNombre { get; set; } = null!;
    public bool EsAprobado { get; set; }
    public bool EsBloqueado { get; set; }
    public DateTime FechaAlta { get; set; }
}
