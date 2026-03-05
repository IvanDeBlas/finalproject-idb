namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class MiInscripcionDto
{
    public Guid Id { get; set; }
    public Guid ProgramaId { get; set; }
    public string ProgramaTitulo { get; set; } = null!;
    public string ArtistaNombre { get; set; } = null!;
    public string TipoPromoNombre { get; set; } = null!;
    public decimal? ImporteComisionPorcentaje { get; set; }
    public decimal? ImporteComisionFija { get; set; }
    public string? MonedaNombre { get; set; }
    public bool EsAprobado { get; set; }
    public bool EsBloqueado { get; set; }
    public string? CodigoReferido { get; set; }
    public string? UrlTrackingPersonalizada { get; set; }
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaBaja { get; set; }
    public string Estado { get; set; } = null!;
    public List<TareaResumenDto> Tareas { get; set; } = new();
}
