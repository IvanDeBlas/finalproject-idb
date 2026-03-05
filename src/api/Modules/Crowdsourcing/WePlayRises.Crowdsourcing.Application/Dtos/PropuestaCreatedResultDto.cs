namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class PropuestaCreatedResultDto
{
    public Guid Id { get; set; }
    public string NecesidadTitulo { get; set; } = null!;
    public decimal PrecioPropuesto { get; set; }
    public string EstadoPropuestaNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
