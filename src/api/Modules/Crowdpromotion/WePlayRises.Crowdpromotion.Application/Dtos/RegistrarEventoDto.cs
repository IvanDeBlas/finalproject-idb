namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class RegistrarEventoDto
{
    public string? CodigoReferido { get; set; }
    public int TipoEventoPromoId { get; set; }
    public Guid? CampaniaCrowdfundingId { get; set; }
    public string? UrlOrigen { get; set; }
    public string? UrlReferer { get; set; }
    public string? UtmSource { get; set; }
    public string? UtmMedium { get; set; }
    public string? UtmCampaign { get; set; }
}
