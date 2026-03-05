namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class MisTareasItemDto
{
    public Guid TareaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? InstruccionesUrl { get; set; }
    public string TipoEventoPromoNombre { get; set; } = null!;
    public string? TipoRewardNombre { get; set; }
    public decimal? ImporteRecompensa { get; set; }
    public string? MonedaNombre { get; set; }
    public int? PuntosRecompensa { get; set; }
    public bool EsRepetible { get; set; }
    public int? MaxRepeticiones { get; set; }
    public int Orden { get; set; }
    public MiEstadoTareaDto? MiEstado { get; set; }
}
