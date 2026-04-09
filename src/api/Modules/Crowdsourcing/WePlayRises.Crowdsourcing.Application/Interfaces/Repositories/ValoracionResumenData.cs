namespace WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;

public class ValoracionResumenData
{
    public decimal? PuntuacionMedia { get; set; }
    public int TotalValoraciones { get; set; }
    public Dictionary<int, int> Distribucion { get; set; } = new();
}
