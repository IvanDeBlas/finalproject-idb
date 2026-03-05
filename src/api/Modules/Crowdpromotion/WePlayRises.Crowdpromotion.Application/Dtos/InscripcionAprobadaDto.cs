namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class InscripcionAprobadaDto
{
    public Guid Id { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public bool EsAprobado { get; set; }
    public bool EsBloqueado { get; set; }
    public string CodigoReferido { get; set; } = null!;
    public string? UrlTrackingPersonalizada { get; set; }
}
