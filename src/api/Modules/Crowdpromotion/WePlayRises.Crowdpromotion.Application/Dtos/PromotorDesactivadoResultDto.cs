namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromotorDesactivadoResultDto
{
    public Guid Id { get; set; }
    public bool EsActivo { get; set; }
    public int ProgramasDadosDeBaja { get; set; }
}
