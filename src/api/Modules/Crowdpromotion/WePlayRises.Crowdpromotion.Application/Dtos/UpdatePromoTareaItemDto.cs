namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class UpdatePromoTareaItemDto
{
    public Guid? Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoEventoPromoId { get; set; }
    public int TipoRewardId { get; set; }
    public decimal? ImporteRecompensa { get; set; }
    public int? MonedaId { get; set; }
    public int? PuntosRecompensa { get; set; }
    public string? UrlInstrucciones { get; set; }
    public bool EsRepetible { get; set; } = true;
    public int? MaxRepeticiones { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public bool EsActivo { get; set; } = true;
}
