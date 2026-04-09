namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// DTO completo para PedidoCrowdfunding (Backing)
/// </summary>
public class PedidoDto
{
    public Guid Id { get; set; }
    public Guid CampaniaId { get; set; }
    public string? UserId { get; set; }
    public Guid? FanProfileId { get; set; }
    public int EstadoPedidoId { get; set; }
    public int MonedaId { get; set; }
    public decimal ImporteSubtotal { get; set; }
    public decimal ImportePropina { get; set; }
    public decimal ImporteEnvio { get; set; }
    public decimal ImporteImpuestos { get; set; }
    public decimal ImporteTotal { get; set; }
    public bool PermitirMostrarNombre { get; set; }
    public string? ComentarioBacker { get; set; }
    public Guid? DireccionEnvioId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public IEnumerable<PedidoLineaDto>? Lineas { get; set; }
}

/// <summary>
/// DTO para lineas de pedido
/// </summary>
public class PedidoLineaDto
{
    public Guid Id { get; set; }
    public Guid RewardId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal ImporteLinea { get; set; }
    public bool EsRewardPrincipal { get; set; }
}
