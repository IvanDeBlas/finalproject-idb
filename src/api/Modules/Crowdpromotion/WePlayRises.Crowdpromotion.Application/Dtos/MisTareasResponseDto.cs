namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class MisTareasResponseDto
{
    public Guid ProgramaId { get; set; }
    public string ProgramaTitulo { get; set; } = null!;
    public List<MisTareasItemDto> Items { get; set; } = new();
}
