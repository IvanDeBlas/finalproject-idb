namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromoProgramaListResultDto
{
    public List<PromoProgramaListItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
