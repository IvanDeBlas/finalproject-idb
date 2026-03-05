namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class RolProfesionalConCategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public CategoriaRolDto CategoriaRol { get; set; } = null!;
    public string? ModalidadCobro { get; set; }
    public bool Activo { get; set; }
}
