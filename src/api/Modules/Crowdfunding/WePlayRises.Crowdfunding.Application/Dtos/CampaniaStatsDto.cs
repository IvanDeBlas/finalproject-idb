namespace WePlayRises.Crowdfunding.Application.Dtos;

public class CampaniaStatsDto
{
    public Guid CampaniaId { get; set; }
    public int TotalBackers { get; set; }
    public decimal TotalRecaudado { get; set; }
    public decimal PromedioAporte { get; set; }
    public decimal AporteMinimo { get; set; }
    public decimal AporteMaximo { get; set; }
    public int DiasRestantes { get; set; }
}
