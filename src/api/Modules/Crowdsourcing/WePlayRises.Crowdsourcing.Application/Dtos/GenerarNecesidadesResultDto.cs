namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class GenerarNecesidadesResultDto
{
    public int NecesidadesCreadas { get; set; }
    public List<Guid> NecesidadIds { get; set; } = new();
    public decimal PresupuestoTotalMin { get; set; }
    public decimal PresupuestoTotalMax { get; set; }
    public int Moneda { get; set; }
}
