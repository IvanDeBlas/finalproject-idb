namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class RankingPromotorItemDto
{
    public Guid PromotorId { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public string? TipoPromotorNombre { get; set; }
    public int Clicks { get; set; }
    public int PageViews { get; set; }
    public int Signups { get; set; }
    public int Conversiones { get; set; }
    public decimal ValorGenerado { get; set; }
    public decimal ComisionAcumulada { get; set; }
}
