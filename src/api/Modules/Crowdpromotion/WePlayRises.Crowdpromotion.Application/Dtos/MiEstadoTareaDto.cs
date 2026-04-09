namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class MiEstadoTareaDto
{
    public Guid TareaPromotorId { get; set; }
    public int EstadoTareaId { get; set; }
    public string EstadoTareaNombre { get; set; } = null!;
    public int VecesCompletada { get; set; }
    public DateTime? FechaPrimeraCompletada { get; set; }
    public DateTime? FechaUltimaCompletada { get; set; }
    public string? UrlPruebaCompletado { get; set; }
    public string? ComentarioValidacion { get; set; }
}
