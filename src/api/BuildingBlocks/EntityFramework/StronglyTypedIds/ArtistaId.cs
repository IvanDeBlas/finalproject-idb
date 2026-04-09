using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for Artista entity
/// </summary>
public readonly record struct ArtistaId(Guid Value) : IComparable<ArtistaId>, IComparable
{
    public static ArtistaId Empty => new(Guid.Empty);
    public static ArtistaId CreateNew() => new(Guid.NewGuid());

    public static ArtistaId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ArtistaId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(ArtistaId id) => id.Value;
    public static explicit operator ArtistaId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static ArtistaId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new ArtistaId(value);

    public int CompareTo(ArtistaId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is ArtistaId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(ArtistaId)}");
    }

    public class EfCoreConverter : ValueConverter<ArtistaId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new ArtistaId(value))
        { }
    }
}
