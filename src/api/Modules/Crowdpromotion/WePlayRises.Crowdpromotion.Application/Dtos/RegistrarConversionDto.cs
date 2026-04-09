namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class RegistrarConversionDto
{
    public string CodigoReferido { get; set; } = null!;
    public Guid CampaniaCrowdfundingId { get; set; }
    public Guid AportacionCrowdfundingId { get; set; }
    public decimal ValorMonetario { get; set; }
    public int MonedaId { get; set; }
    public string UserIdAfectado { get; set; } = null!;
}
