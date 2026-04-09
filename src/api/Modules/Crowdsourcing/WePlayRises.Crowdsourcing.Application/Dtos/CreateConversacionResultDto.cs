namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class CreateConversacionResultDto
{
    public Guid Id { get; set; }
    public string Asunto { get; set; } = null!;
    public string NombreDestinatario { get; set; } = null!;
    public string ContextoTipo { get; set; } = null!;
    public string ContextoTitulo { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
