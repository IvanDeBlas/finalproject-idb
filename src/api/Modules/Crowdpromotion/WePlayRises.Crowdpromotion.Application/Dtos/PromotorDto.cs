namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromotorDto
{
    public Guid Id { get; set; }
    public string NombrePublico { get; set; } = null!;
    public int TipoPromotorId { get; set; }
    public string TipoPromotorNombre { get; set; } = null!;
    public string? EmailContacto { get; set; }
    public string? UrlSitioWeb { get; set; }
    public string? UrlInstagram { get; set; }
    public string? UrlTikTok { get; set; }
    public string? UrlYouTube { get; set; }
    public string? UrlTwitter { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int TotalProgramasActivos { get; set; }
    public decimal TotalComisionesGanadas { get; set; }
    public string MonedaComisiones { get; set; } = "EUR";
}
