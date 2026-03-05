namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class ProgramaExplorarItemDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string ArtistaNombre { get; set; } = null!;
    public int TipoPromoId { get; set; }
    public string TipoPromoNombre { get; set; } = null!;
    public decimal? ImporteComisionPorcentaje { get; set; }
    public decimal? ImporteComisionFija { get; set; }
    public string? MonedaNombre { get; set; }
    public int NumeroTareas { get; set; }
    public string? CampaniaTitulo { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? MiEstado { get; set; }
}
