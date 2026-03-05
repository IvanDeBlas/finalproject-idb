namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class UpdateNecesidadRequest
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int ModalidadTrabajoId { get; set; }
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? UbicacionCiudad { get; set; }
    public string? UbicacionPais { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    public DateTime? FechaInicioPrevista { get; set; }
}
