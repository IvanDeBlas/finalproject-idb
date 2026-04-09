namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class AcuerdoProfesionalDto
{
    public string UserId { get; set; } = null!;
    public Guid? PerfilProfesionalId { get; set; }
    public string Nombre { get; set; } = null!;
}
