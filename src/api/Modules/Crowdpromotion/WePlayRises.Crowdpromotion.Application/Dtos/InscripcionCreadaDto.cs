namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class InscripcionCreadaDto
{
    public Guid Id { get; set; }
    public Guid ProgramaId { get; set; }
    public string ProgramaTitulo { get; set; } = null!;
    public bool EsAprobado { get; set; }
    public bool EsBloqueado { get; set; }
    public DateTime FechaAlta { get; set; }
}
