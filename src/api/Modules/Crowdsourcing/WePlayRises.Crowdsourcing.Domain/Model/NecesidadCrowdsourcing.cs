using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Represents a service need/requirement posted by an artist.
/// </summary>
public class NecesidadCrowdsourcing
{
    public NecesidadCrowdsourcingId Id { get; set; }

    public ProyectoArtisticoId ProyectoArtisticoId { get; set; }

    public ArtistaId ArtistaId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    /// <summary>
    /// Reference to MaestraTipoNecesidad
    /// </summary>
    public int TipoNecesidadId { get; set; }

    /// <summary>
    /// Reference to MaestraEstadoNecesidad
    /// </summary>
    public int EstadoNecesidadId { get; set; }

    /// <summary>
    /// Reference to MaestraModalidadTrabajo
    /// </summary>
    public int ModalidadTrabajoId { get; set; }

    public decimal? PresupuestoMin { get; set; }

    public decimal? PresupuestoMax { get; set; }

    /// <summary>
    /// Reference to MaestraMoneda
    /// </summary>
    public int? MonedaId { get; set; }

    public string? UbicacionCiudad { get; set; }

    public string? UbicacionPais { get; set; }

    public DateTime? FechaLimitePropuestas { get; set; }

    public DateTime? FechaInicioPrevista { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public string? MotivoCierre { get; set; }

    // Navigation properties
    public virtual ICollection<PropuestaCrowdsourcing> Propuestas { get; set; } = new List<PropuestaCrowdsourcing>();
    public virtual ICollection<AcuerdoCrowdsourcing> Acuerdos { get; set; } = new List<AcuerdoCrowdsourcing>();
    public virtual ICollection<ConversacionCrowdsourcing> Conversaciones { get; set; } = new List<ConversacionCrowdsourcing>();
}
