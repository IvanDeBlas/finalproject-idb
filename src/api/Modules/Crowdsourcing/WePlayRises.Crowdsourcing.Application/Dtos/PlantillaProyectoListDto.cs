namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class PlantillaProyectoListDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public decimal PrecioMinTotal { get; set; }
    public decimal PrecioMaxTotal { get; set; }
    public int Moneda { get; set; }
    public int CantidadNecesidades { get; set; }
    public List<string> Fases { get; set; } = new();
}
