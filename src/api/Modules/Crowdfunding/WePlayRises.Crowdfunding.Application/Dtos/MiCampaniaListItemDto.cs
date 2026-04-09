namespace WePlayRises.Crowdfunding.Application.Dtos;

public class MiCampaniaListItemDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? ImagenPrincipalUrl { get; set; }
    public int EstadoCampaniaId { get; set; }
    public string EstadoCampaniaNombre { get; set; } = null!;
    public decimal ImporteObjetivo { get; set; }
    public decimal ImporteRecaudado { get; set; }
    public decimal PorcentajeProgreso { get; set; }
    public int NumBackers { get; set; }
    public int? DiasRestantes { get; set; }
    public DateTime? FechaFin { get; set; }
    public DateTime FechaCreacion { get; set; }
}
