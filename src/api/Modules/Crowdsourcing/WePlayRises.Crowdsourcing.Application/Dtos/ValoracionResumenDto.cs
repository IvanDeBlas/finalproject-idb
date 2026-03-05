namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ValoracionResumenDto
{
    public decimal? PuntuacionMedia { get; set; }
    public int TotalValoraciones { get; set; }
    public Dictionary<int, int> Distribucion { get; set; } = new();
}
