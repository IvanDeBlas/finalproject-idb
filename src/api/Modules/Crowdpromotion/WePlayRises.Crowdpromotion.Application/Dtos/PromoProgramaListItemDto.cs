namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromoProgramaListItemDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int TipoPromoId { get; set; }
    public string TipoPromoNombre { get; set; } = null!;
    public string? CampaniaTitulo { get; set; }
    public bool EsActivo { get; set; }
    public decimal? ImporteComisionPorcentaje { get; set; }
    public decimal? ImporteComisionFija { get; set; }
    public string MonedaNombre { get; set; } = null!;
    public int NumeroPromotores { get; set; }
    public int NumeroTareas { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public DateTime FechaCreacion { get; set; }
}
