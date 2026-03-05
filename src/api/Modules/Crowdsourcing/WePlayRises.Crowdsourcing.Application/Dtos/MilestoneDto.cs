namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class MilestoneDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
    public decimal ImporteParcial { get; set; }
    public decimal PorcentajeParcial { get; set; }
    public DateTime? FechaLimite { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public List<EntregableDto> Entregables { get; set; } = new();
}
