namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ConversacionListResponseDto
{
    public List<ConversacionListItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalNoLeidos { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
