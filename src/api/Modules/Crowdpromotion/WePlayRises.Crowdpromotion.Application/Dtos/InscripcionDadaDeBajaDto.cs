namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class InscripcionDadaDeBajaDto
{
    public Guid Id { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public bool EsAprobado { get; set; }
    public DateTime FechaBaja { get; set; }
}
