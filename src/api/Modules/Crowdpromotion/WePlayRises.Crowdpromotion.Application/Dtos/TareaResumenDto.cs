namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class TareaResumenDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string TipoEventoPromoNombre { get; set; } = null!;
    public decimal? ImporteRecompensa { get; set; }
    public string? MonedaNombre { get; set; }
    public bool EsRepetible { get; set; }
    public int? MaxRepeticiones { get; set; }
    public int Orden { get; set; }
}
