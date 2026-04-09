namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class CreatePromoProgramaRequestDto
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoPromoId { get; set; }
    public Guid? CampaniaCrowdfundingId { get; set; }
    public Guid? ProyectoArtisticoId { get; set; }
    public string? UrlLanding { get; set; }
    public string? CodigoTrackingBase { get; set; }
    public int MonedaId { get; set; }
    public decimal? ImporteComisionPorcentaje { get; set; }
    public decimal? ImporteComisionFija { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public List<CreatePromoTareaItemDto>? Tareas { get; set; }
}
