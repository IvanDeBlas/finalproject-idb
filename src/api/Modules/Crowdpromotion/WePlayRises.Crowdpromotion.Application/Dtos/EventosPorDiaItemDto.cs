namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class EventosPorDiaItemDto
{
    public string Fecha { get; set; } = null!;
    public int Clicks { get; set; }
    public int PageViews { get; set; }
    public int Signups { get; set; }
    public int Conversiones { get; set; }
}
