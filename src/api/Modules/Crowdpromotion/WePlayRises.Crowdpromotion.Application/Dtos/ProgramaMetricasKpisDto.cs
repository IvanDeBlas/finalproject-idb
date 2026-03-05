namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class ProgramaMetricasKpisDto
{
    public int TotalClicks { get; set; }
    public int TotalPageViews { get; set; }
    public int TotalSignups { get; set; }
    public int TotalConversiones { get; set; }
    public decimal ValorTotalGenerado { get; set; }
    public string? MonedaNombre { get; set; }
    public decimal TasaConversion { get; set; }
    public decimal ComisionesTotales { get; set; }
}
