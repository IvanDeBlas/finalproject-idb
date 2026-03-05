namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class CreatePropuestaRequest
{
    public decimal PrecioPropuesto { get; set; }
    public int MonedaId { get; set; }
    public int? DiasEstimados { get; set; }
    public string MensajePropuesta { get; set; } = null!;
}
