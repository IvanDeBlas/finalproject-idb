namespace WePlayRises.Core.Domain.Model;

/// <summary>
/// Base class for all Master (Maestra) tables
/// </summary>
public abstract class BaseMaestra
{
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool EsActivo { get; set; } = true;
    public int Orden { get; set; }
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// Base class for Master tables with Symbol (like currency)
/// </summary>
public abstract class BaseMaestraConSimbolo : BaseMaestra
{
    public string? Simbolo { get; set; }
}
