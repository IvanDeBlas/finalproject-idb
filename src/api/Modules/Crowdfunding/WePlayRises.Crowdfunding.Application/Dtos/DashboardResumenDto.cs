namespace WePlayRises.Crowdfunding.Application.Dtos;

public class DashboardResumenDto
{
    public Guid ArtistaId { get; set; }
    public string NombreArtistico { get; set; } = null!;
    public decimal TotalRecaudado { get; set; }
    public int TotalBackers { get; set; }
    public int CampaniasActivas { get; set; }
    public int CampaniasCompletadas { get; set; }
    public int TotalCampanias { get; set; }
    public string MonedaSimbolo { get; set; } = "EUR";
    public DateTime? FechaUltimoAporte { get; set; }
}
