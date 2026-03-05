namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class InscripcionListItemDto
{
    public Guid Id { get; set; }
    public Guid PromotorId { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public string TipoPromotorNombre { get; set; } = null!;
    public string? PromotorEmailContacto { get; set; }
    public string? PromotorUrlInstagram { get; set; }
    public string? PromotorUrlTikTok { get; set; }
    public string? PromotorUrlSitioWeb { get; set; }
    public bool EsAprobado { get; set; }
    public bool EsBloqueado { get; set; }
    public string? CodigoReferido { get; set; }
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaBaja { get; set; }
    public string Estado { get; set; } = null!;
}
