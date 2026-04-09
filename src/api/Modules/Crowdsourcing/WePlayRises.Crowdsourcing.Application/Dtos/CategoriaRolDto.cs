namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class CategoriaRolDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Icono { get; set; }
    public int Orden { get; set; }
}
