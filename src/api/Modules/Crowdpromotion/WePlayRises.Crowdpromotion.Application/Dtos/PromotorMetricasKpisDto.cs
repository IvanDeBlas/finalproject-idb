namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromotorMetricasKpisDto
{
    public int MisClicks { get; set; }
    public int MisPageViews { get; set; }
    public int MisSignups { get; set; }
    public int MisConversiones { get; set; }
    public decimal MiValorGenerado { get; set; }
    public decimal MiComisionAcumulada { get; set; }
    public string? MonedaNombre { get; set; }
    public decimal MiTasaConversion { get; set; }
}
