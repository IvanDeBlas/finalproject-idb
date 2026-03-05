namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// DTO simplificado para listados de pedidos (backings)
/// </summary>
public class PedidoListDto
{
    public Guid Id { get; set; }
    public Guid CampaniaId { get; set; }
    public string? UserId { get; set; }
    public int EstadoPedidoId { get; set; }
    public decimal ImporteTotal { get; set; }
    public bool PermitirMostrarNombre { get; set; }
    public DateTime FechaCreacion { get; set; }
}
