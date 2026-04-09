namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class MilestoneCreatedResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int Orden { get; set; }
    public decimal ImporteParcial { get; set; }
    public decimal PorcentajeParcial { get; set; }
    public decimal ImporteAsignadoTotal { get; set; }
}
