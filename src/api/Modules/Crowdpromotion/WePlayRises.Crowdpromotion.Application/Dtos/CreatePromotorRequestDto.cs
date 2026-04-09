namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class CreatePromotorRequestDto
{
    public string NombrePublico { get; set; } = null!;
    public int TipoPromotorId { get; set; }
    public string? EmailContacto { get; set; }
    public string? UrlSitioWeb { get; set; }
    public string? UrlInstagram { get; set; }
    public string? UrlTikTok { get; set; }
    public string? UrlYouTube { get; set; }
    public string? UrlTwitter { get; set; }
}
