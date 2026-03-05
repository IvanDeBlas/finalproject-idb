namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class NecesidadCrowdsourcingDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoNecesidadId { get; set; }
    public string TipoNecesidadNombre { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public int ModalidadTrabajoId { get; set; }
    public string ModalidadTrabajoNombre { get; set; } = null!;
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? MonedaNombre { get; set; }
    public string? UbicacionCiudad { get; set; }
    public string? UbicacionPais { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    public DateTime? FechaInicioPrevista { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public Guid ProyectoArtisticoId { get; set; }
    public string ProyectoArtisticoNombre { get; set; } = null!;
    public int NumeroPropuestas { get; set; }
    public List<PropuestaCrowdsourcingDto> Propuestas { get; set; } = new();
}
