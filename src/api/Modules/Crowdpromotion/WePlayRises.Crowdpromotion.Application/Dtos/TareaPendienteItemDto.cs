namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class TareaPendienteItemDto
{
    public Guid TareaPromotorId { get; set; }
    public Guid TareaId { get; set; }
    public string TareaNombre { get; set; } = null!;
    public Guid PromotorId { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public string? PromotorTipoNombre { get; set; }
    public string? UrlPruebaCompletado { get; set; }
    public string? ComentarioPromotor { get; set; }
    public int VecesCompletada { get; set; }
    public DateTime? FechaUltimaCompletada { get; set; }
}
