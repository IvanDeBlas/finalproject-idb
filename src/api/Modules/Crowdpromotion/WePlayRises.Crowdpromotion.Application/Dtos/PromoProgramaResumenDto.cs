namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromoProgramaResumenDto
{
    public int TotalPromotoresAprobados { get; set; }
    public int TotalPromotoresPendientes { get; set; }
    public int TotalEventos { get; set; }
    public int TotalConversiones { get; set; }
    public decimal ValorTotalGenerado { get; set; }
}
