namespace WePlayRises.Crowdfunding.Application.Dtos;

public class BackingPublicDto
{
    public Guid Id { get; set; }
    public string NombreBacker { get; set; } = null!;
    public decimal Monto { get; set; }
    public string? RewardNombre { get; set; }
    public string? Mensaje { get; set; }
    public DateTime FechaCreacion { get; set; }
}
