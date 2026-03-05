namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class InscripcionBloqueadaDto
{
    public Guid Id { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public bool EsBloqueado { get; set; }
    public bool EsAprobado { get; set; }
}
