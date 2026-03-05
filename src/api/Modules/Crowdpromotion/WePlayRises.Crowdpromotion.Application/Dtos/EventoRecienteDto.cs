namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class EventoRecienteDto
{
    public Guid Id { get; set; }
    public int TipoEventoId { get; set; }
    public string TipoEventoNombre { get; set; } = null!;
    public decimal ValorMonetario { get; set; }
    public decimal? ComisionGenerada { get; set; }
    public string? MonedaNombre { get; set; }
    public DateTime FechaEvento { get; set; }
}
