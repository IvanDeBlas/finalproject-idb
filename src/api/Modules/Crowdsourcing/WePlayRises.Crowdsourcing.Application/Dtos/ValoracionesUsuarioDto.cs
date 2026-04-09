namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ValoracionesUsuarioDto
{
    public ValoracionResumenDto Resumen { get; set; } = null!;
    public PaginatedResponse<ValoracionListItemDto> Valoraciones { get; set; } = null!;
}
