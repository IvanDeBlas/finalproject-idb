namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class TareasPendientesResponseDto
{
    public List<TareaPendienteItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
