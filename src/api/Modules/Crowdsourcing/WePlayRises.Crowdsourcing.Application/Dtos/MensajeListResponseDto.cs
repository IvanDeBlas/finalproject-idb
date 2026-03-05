namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class MensajeListResponseDto
{
    public List<MensajeDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
