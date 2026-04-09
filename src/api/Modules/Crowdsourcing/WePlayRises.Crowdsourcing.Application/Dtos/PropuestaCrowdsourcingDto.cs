namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class PropuestaCrowdsourcingDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string? MensajePropuesta { get; set; }
    public decimal PrecioPropuesto { get; set; }
    public int? MonedaId { get; set; }
    public int? DiasEstimados { get; set; }
    public int EstadoPropuestaId { get; set; }
    public string EstadoPropuestaNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
