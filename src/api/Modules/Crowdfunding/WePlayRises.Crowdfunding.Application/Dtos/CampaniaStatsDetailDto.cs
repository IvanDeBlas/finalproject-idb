namespace WePlayRises.Crowdfunding.Application.Dtos;

public class CampaniaStatsDetailDto
{
    public Guid CampaniaId { get; set; }
    public string CampaniaTitulo { get; set; } = null!;
    public decimal ImporteObjetivo { get; set; }
    public decimal ImporteRecaudado { get; set; }
    public decimal PorcentajeProgreso { get; set; }
    public int NumBackers { get; set; }
    public decimal BackingPromedio { get; set; }
    public int? DiasRestantes { get; set; }
    public int DiasTranscurridos { get; set; }
    public int TotalDiasCampania { get; set; }
    public decimal? ProyeccionFinal { get; set; }
    public decimal VelocidadDiaria { get; set; }
    public List<RewardStatDto> RewardStats { get; set; } = new();
    public List<ProgressoDiaDto> ProgressoPorDia { get; set; } = new();
}

public class RewardStatDto
{
    public Guid? RewardId { get; set; }
    public string RewardNombre { get; set; } = null!;
    public int CantidadVendida { get; set; }
    public decimal TotalRecaudado { get; set; }
    public decimal PorcentajeDelTotal { get; set; }
}

public class ProgressoDiaDto
{
    public string Fecha { get; set; } = null!;
    public int NumBackings { get; set; }
    public decimal TotalRecaudado { get; set; }
    public decimal Acumulado { get; set; }
}
