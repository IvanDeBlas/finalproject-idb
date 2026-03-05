namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class NecesidadPublicaListDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoNecesidadId { get; set; }
    public string TipoNecesidadNombre { get; set; } = null!;
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? MonedaNombre { get; set; }
    public int ModalidadTrabajoId { get; set; }
    public string ModalidadTrabajoNombre { get; set; } = null!;
    public string? UbicacionCiudad { get; set; }
    public string? UbicacionPais { get; set; }
    public string ArtistaNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    public bool EsUrgente { get; set; }
    public int NumeroPropuestas { get; set; }
}
