namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class PlantillaProyectoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public List<PlantillaProyectoNecesidadDto> Necesidades { get; set; } = new();
    public PlantillaResumenDto Resumen { get; set; } = null!;
}
