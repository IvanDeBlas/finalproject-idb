namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromoProgramaDesactivadoResultDto
{
    public Guid Id { get; set; }
    public bool EsActivo { get; set; }
    public int TareasDesactivadas { get; set; }
}
