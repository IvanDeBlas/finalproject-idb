namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class UpdatePromotorRequestDto
{
    public string NombrePublico { get; set; } = null!;
    public string? EmailContacto { get; set; }
    public string? UrlSitioWeb { get; set; }
    public string? UrlInstagram { get; set; }
    public string? UrlTikTok { get; set; }
    public string? UrlYouTube { get; set; }
    public string? UrlTwitter { get; set; }
}
