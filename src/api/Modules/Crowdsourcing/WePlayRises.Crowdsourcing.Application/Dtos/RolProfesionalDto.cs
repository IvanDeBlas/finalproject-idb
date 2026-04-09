namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class RolProfesionalDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int CategoriaRolId { get; set; }
    public string? ModalidadCobro { get; set; }
}
