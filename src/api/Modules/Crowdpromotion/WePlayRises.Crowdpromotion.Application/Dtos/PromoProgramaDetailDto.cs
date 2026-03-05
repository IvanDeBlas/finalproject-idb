namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromoProgramaDetailDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoPromoId { get; set; }
    public string TipoPromoNombre { get; set; } = null!;
    public Guid? CampaniaCrowdfundingId { get; set; }
    public string? CampaniaTitulo { get; set; }
    public Guid? ProyectoArtisticoId { get; set; }
    public string? UrlLanding { get; set; }
    public string? CodigoTrackingBase { get; set; }
    public int MonedaId { get; set; }
    public string MonedaNombre { get; set; } = null!;
    public decimal? ImporteComisionPorcentaje { get; set; }
    public decimal? ImporteComisionFija { get; set; }
    public bool EsActivo { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public List<PromoTareaDetailDto> Tareas { get; set; } = new();
    public List<PromoProgramaPromotorSummaryDto> Promotores { get; set; } = new();
    public PromoProgramaResumenDto Resumen { get; set; } = new();
}
