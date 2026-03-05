namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class MiPropuestaListDto
{
    public Guid Id { get; set; }
    public string NecesidadTitulo { get; set; } = null!;
    public string ArtistaNombre { get; set; } = null!;
    public decimal PrecioPropuesto { get; set; }
    public int? MonedaId { get; set; }
    public string MonedaNombre { get; set; } = null!;
    public int EstadoPropuestaId { get; set; }
    public string EstadoPropuestaNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public Guid? AcuerdoId { get; set; }
}
