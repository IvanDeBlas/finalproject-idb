namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ValoracionCreatedResultDto
{
    public Guid Id { get; set; }
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
    public DateTime FechaCreacion { get; set; }
}
