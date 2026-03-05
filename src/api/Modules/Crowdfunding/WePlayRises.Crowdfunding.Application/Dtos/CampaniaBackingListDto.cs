namespace WePlayRises.Crowdfunding.Application.Dtos;

public class CampaniaBackingListDto
{
    public Guid CampaniaId { get; set; }
    public string CampaniaTitulo { get; set; } = null!;
    public CampaniaBackingStatsDto Stats { get; set; } = null!;
    public PaginatedResponse<CampaniaBackingItemDto> Backings { get; set; } = null!;
}

public class CampaniaBackingStatsDto
{
    public decimal TotalRecaudado { get; set; }
    public decimal BackingPromedio { get; set; }
    public int TotalBackers { get; set; }
    public string? RewardMasPopular { get; set; }
    public UltimoBackingDto? UltimoBacking { get; set; }
}

public class UltimoBackingDto
{
    public string NombreBacker { get; set; } = null!;
    public decimal Monto { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class CampaniaBackingItemDto
{
    public Guid Id { get; set; }
    public string NombreBacker { get; set; } = null!;
    public string? Email { get; set; }
    public decimal Monto { get; set; }
    public string? RewardNombre { get; set; }
    public string? Mensaje { get; set; }
    public bool EsAnonimo { get; set; }
    public string EstadoPedido { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
