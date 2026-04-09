namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// Request DTO for updating a campaign. Does NOT include ArtistaId (extracted from JWT token).
/// </summary>
public class UpdateCampaniaRequest
{
    public Guid Id { get; set; }
    public string? Titulo { get; set; }
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public decimal? ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int? TipoFinanciacionId { get; set; }
    public bool? PermiteAportacionesAnonimas { get; set; }
    public bool? PermitePropinas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
