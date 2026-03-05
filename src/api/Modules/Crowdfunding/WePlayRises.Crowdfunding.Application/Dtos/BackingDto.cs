namespace WePlayRises.Crowdfunding.Application.Dtos;

public class BackingDto
{
    public Guid Id { get; set; }
    public Guid CampaniaId { get; set; }
    public string CampaniaTitulo { get; set; } = null!;
    public Guid? RewardId { get; set; }
    public string? RewardNombre { get; set; }
    public decimal Monto { get; set; }
    public string MonedaSimbolo { get; set; } = null!;
    public string? Mensaje { get; set; }
    public bool EsAnonimo { get; set; }
    public string? UserName { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string EstadoPedido { get; set; } = null!;
}
