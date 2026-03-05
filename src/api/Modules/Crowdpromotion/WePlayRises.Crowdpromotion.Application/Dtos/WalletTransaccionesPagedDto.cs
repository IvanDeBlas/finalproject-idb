namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class WalletTransaccionesPagedDto
{
    public List<WalletTransaccionItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
