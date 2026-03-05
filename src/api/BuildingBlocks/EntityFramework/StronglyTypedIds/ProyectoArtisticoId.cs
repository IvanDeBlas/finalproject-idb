using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for ProyectoArtistico entity
/// </summary>
public readonly record struct ProyectoArtisticoId(Guid Value) : IComparable<ProyectoArtisticoId>, IComparable
{
    public static ProyectoArtisticoId Empty => new(Guid.Empty);
    public static ProyectoArtisticoId CreateNew() => new(Guid.NewGuid());

    public static ProyectoArtisticoId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ProyectoArtisticoId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(ProyectoArtisticoId id) => id.Value;
    public static explicit operator ProyectoArtisticoId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static ProyectoArtisticoId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new ProyectoArtisticoId(value);

    public int CompareTo(ProyectoArtisticoId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is ProyectoArtisticoId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(ProyectoArtisticoId)}");
    }

    public class EfCoreConverter : ValueConverter<ProyectoArtisticoId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new ProyectoArtisticoId(value))
        { }
    }
}
