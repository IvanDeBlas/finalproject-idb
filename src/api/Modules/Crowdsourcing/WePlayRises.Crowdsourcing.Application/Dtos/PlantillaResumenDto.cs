namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class PlantillaResumenDto
{
    public decimal PrecioMinTotal { get; set; }
    public decimal PrecioMaxTotal { get; set; }
    public int Moneda { get; set; }
    public int CantidadNecesidadesAlta { get; set; }
    public int CantidadNecesidadesMedia { get; set; }
    public int CantidadNecesidadesBaja { get; set; }
}
