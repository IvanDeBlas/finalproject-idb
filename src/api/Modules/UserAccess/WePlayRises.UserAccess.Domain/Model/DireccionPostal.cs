namespace WePlayRises.UserAccess.Domain.Model;

/// <summary>
/// Represents a postal/shipping address.
/// </summary>
public class DireccionPostal
{
    public Guid Id { get; set; }

    /// <summary>
    /// Reference to the Identity User (optional)
    /// </summary>
    public string? UserId { get; set; }

    public string NombreDestinatario { get; set; } = null!;

    public string Linea1 { get; set; } = null!;

    public string? Linea2 { get; set; }

    public string Ciudad { get; set; } = null!;

    public string? Provincia { get; set; }

    public string CodigoPostal { get; set; } = null!;

    public string? Pais { get; set; }

    public string? Telefono { get; set; }

    public DateTime FechaCreacion { get; set; }
}
