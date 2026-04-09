namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class PlantillaProyectoNecesidadDto
{
    public Guid Id { get; set; }
    public string Fase { get; set; } = null!;
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public RolProfesionalDto RolProfesional { get; set; } = null!;
    public decimal? PrecioMinOrientativo { get; set; }
    public decimal? PrecioMaxOrientativo { get; set; }
    public int Moneda { get; set; }
    public string Prioridad { get; set; } = null!;
    public int Orden { get; set; }
}
